using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface ISavedWordRepository
{
    Task<IReadOnlyList<SavedWord>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid userId, Guid wordId, CancellationToken ct = default);
    Task<SavedWord?> GetAsync(Guid userId, Guid wordId, CancellationToken ct = default);
    Task AddAsync(SavedWord savedWord, CancellationToken ct = default);
    Task RemoveAsync(SavedWord savedWord, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}