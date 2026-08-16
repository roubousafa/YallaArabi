using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IPointTransactionRepository
{
    Task<int> GetTotalPointsAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<PointTransaction>> GetByUserIdAsync(Guid userId, int maxResults, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid userId, string actionKey, Guid relatedEntityId, CancellationToken ct = default);
    Task AddAsync(PointTransaction transaction, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}