using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface ILessonContentRepository
{
    Task<IReadOnlyList<LessonWord>> GetWordsAsync(Guid lessonId, CancellationToken ct = default);
    Task<LessonWord?> GetWordAsync(Guid lessonId, Guid wordId, CancellationToken ct = default);
    Task AddWordsAsync(IEnumerable<LessonWord> words, CancellationToken ct = default);
    void RemoveWord(LessonWord word);

    Task<IReadOnlyList<LessonQuestion>> GetQuestionsAsync(Guid lessonId, CancellationToken ct = default);
    Task<LessonQuestion?> GetQuestionByIdAsync(Guid questionId, CancellationToken ct = default);
    Task<int> CountQuestionsAsync(Guid lessonId, CancellationToken ct = default);
    Task AddQuestionAsync(LessonQuestion question, CancellationToken ct = default);
    void RemoveQuestion(LessonQuestion question);

    Task RemoveAllForLessonAsync(Guid lessonId, CancellationToken ct = default);
    Task<bool> IsWordUsedInQuestionsAsync(Guid wordId, CancellationToken ct = default);
    Task RemoveWordFromAllLessonsAsync(Guid wordId, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
