using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IChallengeParticipationRepository
{
    Task<ChallengeParticipation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ChallengeParticipation?> GetActiveAsync(Guid userId, Guid challengeId, CancellationToken ct = default);
    Task<bool> HasCompletedAsync(Guid userId, Guid challengeId, CancellationToken ct = default);
    Task<IReadOnlyList<ChallengeParticipation>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<ChallengeParticipation>> GetByChallengeIdAsync(Guid challengeId, CancellationToken ct = default);
    Task AddAsync(ChallengeParticipation participation, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}