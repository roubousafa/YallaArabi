using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _context;

    public SubscriptionRepository(AppDbContext context) => _context = context;

    public async Task<Subscription?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Subscriptions
            .Where(s => s.UserId == userId && s.Status == "active")
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(Subscription subscription, CancellationToken ct = default) =>
        await _context.Subscriptions.AddAsync(subscription, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<(IReadOnlyList<Subscription> Items, int TotalCount)> GetPagedAsync(
        string? status, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Subscriptions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(s => s.Status == status);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(s => s.StartedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}