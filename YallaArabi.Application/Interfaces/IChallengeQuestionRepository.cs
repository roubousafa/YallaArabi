using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IChallengeQuestionRepository
{
    Task<ChallengeQuestion?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ChallengeQuestion>> GetByChallengeIdAsync(Guid challengeId, CancellationToken ct = default);
    Task<int> CountByChallengeIdAsync(Guid challengeId, CancellationToken ct = default);
    Task AddAsync(ChallengeQuestion question, CancellationToken ct = default);
    Task RemoveAsync(ChallengeQuestion question, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}