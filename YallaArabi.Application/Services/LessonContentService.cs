using YallaArabi.Application.Features.Levels.GetLessonContent;
using YallaArabi.Application.Features.Words.SearchWords;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Services;

// Builds the full lesson payload (words + questions) shared by the student and admin endpoints.
public sealed class LessonContentService : ILessonContentService
{
    private readonly ILessonContentRepository _contentRepository;
    private readonly IWordRepository _wordRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly IWordReviewRepository _wordReviewRepository;

    public LessonContentService(
        ILessonContentRepository contentRepository,
        IWordRepository wordRepository,
        ILevelRepository levelRepository,
        IWordReviewRepository wordReviewRepository)
    {
        _contentRepository = contentRepository;
        _wordRepository = wordRepository;
        _levelRepository = levelRepository;
        _wordReviewRepository = wordReviewRepository;
    }

    public async Task<LessonContentResponse> BuildAsync(Lesson lesson, bool isCompleted, Guid? studentId, CancellationToken ct = default)
    {
        var lessonWords = await _contentRepository.GetWordsAsync(lesson.Id, ct);
        var questions = await _contentRepository.GetQuestionsAsync(lesson.Id, ct);

        var wordIds = lessonWords.Select(w => w.WordId)
            .Concat(questions.Where(q => q.WordId.HasValue).Select(q => q.WordId!.Value));

        var words = (await _wordRepository.GetByIdsAsync(wordIds, ct)).ToDictionary(w => w.Id);
        var levelCodes = (await _levelRepository.GetAllAsync(ct)).ToDictionary(l => l.Id, l => l.Code);

        WordSearchResult? ToResult(Guid? wordId) =>
            wordId is Guid id && words.TryGetValue(id, out var word)
                ? new WordSearchResult(
                    word.Id,
                    word.ArabicText,
                    word.Transliteration,
                    word.EnglishMeaning,
                    levelCodes.GetValueOrDefault(word.LevelId, "?"),
                    word.AudioUrl,
                    word.ImageUrl)
                : null;

        var wordResults = lessonWords
            .Select(lw => ToResult(lw.WordId))
            .OfType<WordSearchResult>()
            .ToList();

        var wordProgress = new List<WordProgressItem>();

        if (studentId.HasValue)
        {
            var reviews = (await _wordReviewRepository.GetForWordsAsync(studentId.Value, wordResults.Select(w => w.WordId), ct))
                .ToDictionary(r => r.WordId);

            wordProgress = wordResults
                .Select(w =>
                {
                    var review = reviews.GetValueOrDefault(w.WordId);
                    return new WordProgressItem(w.WordId, WordMastery.Of(review), review?.NextReviewAt);
                })
                .ToList();
        }

        var questionItems = questions
            .Select(q =>
            {
                var word = ToResult(q.WordId);

                return new LessonQuestionItem(
                    q.Id,
                    q.Type,
                    q.QuestionText,
                    BuildOptions(q),
                    q.CorrectOption,
                    word,
                    q.AudioUrl ?? word?.AudioUrl,
                    q.ImageUrl ?? word?.ImageUrl,
                    q.OrderIndex);
            })
            .ToList();

        return new LessonContentResponse(
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.IconUrl,
            lesson.PointsReward,
            isCompleted,
            wordResults,
            wordProgress,
            questionItems);
    }

    private static IReadOnlyList<QuestionOption> BuildOptions(LessonQuestion question)
    {
        var options = new List<QuestionOption>();

        void Add(string key, string? text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                options.Add(new QuestionOption(key, text));
            }
        }

        Add("A", question.OptionA);
        Add("B", question.OptionB);
        Add("C", question.OptionC);
        Add("D", question.OptionD);

        return options;
    }
}
