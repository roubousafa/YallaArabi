using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.AddLessonQuestion;

public sealed class AddLessonQuestionCommandHandler : IRequestHandler<AddLessonQuestionCommand, Result<AddLessonQuestionResponse>>
{
    private const int MaxQuestionsPerLesson = 50;

    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonContentRepository _contentRepository;
    private readonly IWordRepository _wordRepository;

    public AddLessonQuestionCommandHandler(
        ILessonRepository lessonRepository,
        ILessonContentRepository contentRepository,
        IWordRepository wordRepository)
    {
        _lessonRepository = lessonRepository;
        _contentRepository = contentRepository;
        _wordRepository = wordRepository;
    }

    public async ValueTask<Result<AddLessonQuestionResponse>> Handle(AddLessonQuestionCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result<AddLessonQuestionResponse>.Failure("lesson_not_found", "Lesson not found");
        }

        if (request.WordId.HasValue && await _wordRepository.GetByIdAsync(request.WordId.Value, cancellationToken) is null)
        {
            return Result<AddLessonQuestionResponse>.Failure("word_not_found", "Word not found");
        }

        var currentCount = await _contentRepository.CountQuestionsAsync(lesson.Id, cancellationToken);

        if (currentCount >= MaxQuestionsPerLesson)
        {
            return Result<AddLessonQuestionResponse>.Failure(
                "max_questions_reached",
                $"A lesson cannot have more than {MaxQuestionsPerLesson} questions");
        }

        var question = LessonQuestion.Create(
            lesson.Id,
            request.Type,
            request.QuestionText,
            request.OptionA,
            request.OptionB,
            request.OptionC,
            request.OptionD,
            request.CorrectOption,
            request.WordId,
            request.AudioUrl,
            request.ImageUrl,
            currentCount);

        await _contentRepository.AddQuestionAsync(question, cancellationToken);
        await _contentRepository.SaveChangesAsync(cancellationToken);

        return Result<AddLessonQuestionResponse>.Success(
            new AddLessonQuestionResponse(question.Id, lesson.Id, question.OrderIndex, currentCount + 1));
    }
}
