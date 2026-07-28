using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IWordReviewRepository
{
    Task<WordReview?> GetAsync(Guid userId, Guid wordId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid userId, Guid wordId, CancellationToken ct = default);
    Task<IReadOnlyList<WordReview>> GetDueReviewsAsync(Guid userId, DateTime asOf, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetUserIdsWithOverdueReviewsAsync(DateTime overdueBefore, CancellationToken ct = default);
    Task AddAsync(WordReview review, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}