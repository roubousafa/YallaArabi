using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class ChallengeRepository : IChallengeRepository
{
    private readonly AppDbContext _context;

    public ChallengeRepository(AppDbContext context) => _context = context;

    public async Task<Challenge?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Challenges.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<(IReadOnlyList<Challenge> Items, int TotalCount)> GetPagedAsync(
        string? status, Guid? levelId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Challenges.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(c => c.Status == status);
        }

        if (levelId.HasValue)
        {
            query = query.Where(c => c.LevelId == levelId.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Challenge>> GetAvailableForLevelAsync(Guid? levelId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var query = _context.Challenges
            .Where(c => c.Status == "published")
            .Where(c => c.ExpiresAt == null || c.ExpiresAt > now);

        if (levelId.HasValue)
        {
            query = query.Where(c => c.LevelId == null || c.LevelId == levelId.Value);
        }
        else
        {
            query = query.Where(c => c.LevelId == null);
        }

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync(ct);
    }

    public async Task AddAsync(Challenge challenge, CancellationToken ct = default) =>
        await _context.Challenges.AddAsync(challenge, ct);

    public async Task RemoveAsync(Challenge challenge, CancellationToken ct = default)
    {
        _context.Challenges.Remove(challenge);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}