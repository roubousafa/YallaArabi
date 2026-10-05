using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IUserNotificationRepository
{
    Task<(IReadOnlyList<UserNotification> Items, int TotalCount)> GetPagedAsync(Guid userId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<int> CountUnreadAsync(Guid userId, CancellationToken ct = default);
    Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsSinceAsync(Guid userId, string type, DateTime sinceUtc, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<UserNotification> notifications, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
