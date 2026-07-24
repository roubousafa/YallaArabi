using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IStudentLessonProgressRepository
{
    Task<IReadOnlyList<StudentLessonProgress>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid userId, Guid lessonId, CancellationToken ct = default);
    Task AddAsync(StudentLessonProgress progress, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}