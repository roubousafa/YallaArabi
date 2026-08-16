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

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}