using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context) => _context = context;

    public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Notifications.AsQueryable();

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(n => n.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(Notification notification, CancellationToken ct = default) =>
        await _context.Notifications.AddAsync(notification, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}