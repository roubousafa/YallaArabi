using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IChallengeRepository
{
    Task<Challenge?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Challenge> Items, int TotalCount)> GetPagedAsync(string? status, Guid? levelId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<Challenge>> GetAvailableForLevelAsync(Guid? levelId, CancellationToken ct = default);
    Task AddAsync(Challenge challenge, CancellationToken ct = default);
    Task RemoveAsync(Challenge challenge, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}