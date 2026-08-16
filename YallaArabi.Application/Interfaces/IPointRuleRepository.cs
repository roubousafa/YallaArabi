using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IPointRuleRepository
{
    Task<IReadOnlyList<PointRule>> GetAllAsync(CancellationToken ct = default);
    Task<PointRule?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PointRule?> GetByActionKeyAsync(string actionKey, CancellationToken ct = default);
    Task<bool> ActionKeyExistsAsync(string actionKey, CancellationToken ct = default);
    Task AddAsync(PointRule rule, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}