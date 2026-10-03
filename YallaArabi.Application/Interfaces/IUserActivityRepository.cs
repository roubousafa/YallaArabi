using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IUserActivityRepository
{
    Task<bool> ExistsAsync(Guid userId, string activityType, Guid relatedEntityId, DateOnly localDate, CancellationToken ct = default);
    Task<int> GetTotalSecondsAsync(Guid userId, DateOnly localDate, CancellationToken ct = default);
    Task<IReadOnlyList<DateOnly>> GetActiveDatesAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(UserActivity activity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
