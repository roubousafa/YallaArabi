using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface ILessonRepository
{
    Task<IReadOnlyList<Lesson>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default);
    Task<IReadOnlyList<Lesson>> GetByCategoryIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken ct = default);
    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<int> CountByCategoryIdAsync(Guid categoryId, CancellationToken ct = default);
    Task<int> CountCompletionsAsync(Guid lessonId, CancellationToken ct = default);
    Task AddAsync(Lesson lesson, CancellationToken ct = default);
    Task RemoveAsync(Lesson lesson, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}