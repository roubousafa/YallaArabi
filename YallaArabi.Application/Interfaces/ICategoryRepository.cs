using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetByLevelIdAsync(Guid levelId, CancellationToken ct = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<int> CountByLevelIdAsync(Guid levelId, CancellationToken ct = default);
    Task<int> CountLessonsAsync(Guid categoryId, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    Task RemoveAsync(Category category, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}