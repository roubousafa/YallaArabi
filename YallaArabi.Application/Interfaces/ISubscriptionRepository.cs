using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Subscription subscription, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}