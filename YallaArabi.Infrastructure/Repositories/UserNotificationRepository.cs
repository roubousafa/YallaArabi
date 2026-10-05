using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class UserNotificationRepository : IUserNotificationRepository
{
    private readonly AppDbContext _context;

    public UserNotificationRepository(AppDbContext context) => _context = context;

    public async Task<(IReadOnlyList<UserNotification> Items, int TotalCount)> GetPagedAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.UserNotifications.Where(n => n.UserId == userId);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.UserNotifications.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<int> CountUnreadAsync(Guid userId, CancellationToken ct = default) =>
        await _context.UserNotifications.CountAsync(n => n.UserId == userId && n.ReadAt == null, ct);

    public async Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken ct = default) =>
        await _context.UserNotifications
            .Where(n => n.UserId == userId && n.ReadAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.ReadAt, DateTime.UtcNow), ct);

    public async Task<bool> ExistsSinceAsync(Guid userId, string type, DateTime sinceUtc, CancellationToken ct = default) =>
        await _context.UserNotifications.AnyAsync(n => n.UserId == userId && n.Type == type && n.CreatedAt >= sinceUtc, ct);

    public async Task AddRangeAsync(IEnumerable<UserNotification> notifications, CancellationToken ct = default) =>
        await _context.UserNotifications.AddRangeAsync(notifications, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}
