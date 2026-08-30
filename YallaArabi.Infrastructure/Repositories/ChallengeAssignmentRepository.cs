using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class ChallengeAssignmentRepository : IChallengeAssignmentRepository
{
    private readonly AppDbContext _context;

    public ChallengeAssignmentRepository(AppDbContext context) => _context = context;

    public async Task<bool> ExistsAsync(Guid challengeId, Guid userId, CancellationToken ct = default) =>
        await _context.ChallengeAssignments.AnyAsync(a => a.ChallengeId == challengeId && a.UserId == userId, ct);

    public async Task<bool> HasAnyAssignmentsAsync(Guid challengeId, CancellationToken ct = default) =>
        await _context.ChallengeAssignments.AnyAsync(a => a.ChallengeId == challengeId, ct);

    public async Task<IReadOnlyList<Guid>> GetAssignedChallengeIdsForUserAsync(Guid userId, CancellationToken ct = default) =>
        await _context.ChallengeAssignments
            .Where(a => a.UserId == userId)
            .Select(a => a.ChallengeId)
            .Distinct()
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Guid>> GetChallengeIdsWithAssignmentsAsync(CancellationToken ct = default) =>
        await _context.ChallengeAssignments
            .Select(a => a.ChallengeId)
            .Distinct()
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ChallengeAssignment>> GetByChallengeIdAsync(Guid challengeId, CancellationToken ct = default) =>
        await _context.ChallengeAssignments
            .Where(a => a.ChallengeId == challengeId)
            .ToListAsync(ct);

    public async Task AddAsync(ChallengeAssignment assignment, CancellationToken ct = default) =>
        await _context.ChallengeAssignments.AddAsync(assignment, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}