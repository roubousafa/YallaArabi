using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class ChallengeParticipationRepository : IChallengeParticipationRepository
{
    private readonly AppDbContext _context;

    public ChallengeParticipationRepository(AppDbContext context) => _context = context;

    public async Task<ChallengeParticipation?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.ChallengeParticipations.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<ChallengeParticipation?> GetActiveAsync(Guid userId, Guid challengeId, CancellationToken ct = default) =>
        await _context.ChallengeParticipations
            .Where(p => p.UserId == userId && p.ChallengeId == challengeId && p.Status == "in_progress")
            .OrderByDescending(p => p.StartedAt)
            .FirstOrDefaultAsync(ct);

    public async Task<bool> HasCompletedAsync(Guid userId, Guid challengeId, CancellationToken ct = default) =>
        await _context.ChallengeParticipations
            .AnyAsync(p => p.UserId == userId && p.ChallengeId == challengeId && p.Status == "completed", ct);

    public async Task<IReadOnlyList<ChallengeParticipation>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.ChallengeParticipations
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.StartedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ChallengeParticipation>> GetByChallengeIdAsync(Guid challengeId, CancellationToken ct = default) =>
        await _context.ChallengeParticipations
            .Where(p => p.ChallengeId == challengeId)
            .OrderByDescending(p => p.CorrectAnswers)
            .ToListAsync(ct);

    public async Task AddAsync(ChallengeParticipation participation, CancellationToken ct = default) =>
        await _context.ChallengeParticipations.AddAsync(participation, ct);

    public async Task<(IReadOnlyList<ChallengeParticipation> Items, int TotalCount)> GetReportAsync(
    Guid? userId, Guid? challengeId, IReadOnlyList<Guid>? challengeIds,
    int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = BuildReportQuery(userId, challengeId, challengeIds);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.StartedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<ChallengeParticipation>> GetReportForExportAsync(
        Guid? userId, Guid? challengeId, IReadOnlyList<Guid>? challengeIds,
        int maxRecords, CancellationToken ct = default)
    {
        return await BuildReportQuery(userId, challengeId, challengeIds)
            .OrderByDescending(p => p.StartedAt)
            .Take(maxRecords)
            .ToListAsync(ct);
    }

    private IQueryable<ChallengeParticipation> BuildReportQuery(
        Guid? userId, Guid? challengeId, IReadOnlyList<Guid>? challengeIds)
    {
        var query = _context.ChallengeParticipations.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(p => p.UserId == userId.Value);
        }

        if (challengeId.HasValue)
        {
            query = query.Where(p => p.ChallengeId == challengeId.Value);
        }
        else if (challengeIds is not null)
        {
            query = query.Where(p => challengeIds.Contains(p.ChallengeId));
        }

        return query;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}