using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class UserActivityRepository : IUserActivityRepository
{
    private readonly AppDbContext _context;

    public UserActivityRepository(AppDbContext context) => _context = context;

    public async Task<bool> ExistsAsync(Guid userId, string activityType, Guid relatedEntityId, DateOnly localDate, CancellationToken ct = default) =>
        await _context.UserActivities
            .AnyAsync(a => a.UserId == userId
                && a.ActivityType == activityType
                && a.RelatedEntityId == relatedEntityId
                && a.LocalDate == localDate, ct);

    public async Task<int> GetTotalSecondsAsync(Guid userId, DateOnly localDate, CancellationToken ct = default) =>
        await _context.UserActivities
            .Where(a => a.UserId == userId && a.LocalDate == localDate)
            .SumAsync(a => a.DurationSeconds, ct);

    public async Task<IReadOnlyList<DateOnly>> GetActiveDatesAsync(Guid userId, CancellationToken ct = default) =>
        await _context.UserActivities
            .Where(a => a.UserId == userId)
            .Select(a => a.LocalDate)
            .Distinct()
            .OrderByDescending(d => d)
            .ToListAsync(ct);

    public async Task AddAsync(UserActivity activity, CancellationToken ct = default) =>
        await _context.UserActivities.AddAsync(activity, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}
