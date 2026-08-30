using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IChallengeAssignmentRepository
{
    Task<bool> ExistsAsync(Guid challengeId, Guid userId, CancellationToken ct = default);
    Task<bool> HasAnyAssignmentsAsync(Guid challengeId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetAssignedChallengeIdsForUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetChallengeIdsWithAssignmentsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ChallengeAssignment>> GetByChallengeIdAsync(Guid challengeId, CancellationToken ct = default);
    Task AddAsync(ChallengeAssignment assignment, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}