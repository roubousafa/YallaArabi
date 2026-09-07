using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IChallengeAnswerRepository
{
    Task<IReadOnlyList<ChallengeAnswer>> GetByParticipationIdAsync(Guid participationId, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<ChallengeAnswer> answers, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}