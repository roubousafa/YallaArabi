using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class WordReviewRepository : IWordReviewRepository
{
    private readonly AppDbContext _context;

    public WordReviewRepository(AppDbContext context) => _context = context;

    public async Task<WordReview?> GetAsync(Guid userId, Guid wordId, CancellationToken ct = default) =>
        await _context.WordReviews.FirstOrDefaultAsync(w => w.UserId == userId && w.WordId == wordId, ct);

    public async Task<IReadOnlyList<WordReview>> GetForWordsAsync(Guid userId, IEnumerable<Guid> wordIds, CancellationToken ct = default)
    {
        var ids = wordIds.Distinct().ToList();
        return await _context.WordReviews.Where(r => r.UserId == userId && ids.Contains(r.WordId)).ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid wordId, CancellationToken ct = default) =>
        await _context.WordReviews.AnyAsync(w => w.UserId == userId && w.WordId == wordId, ct);

    public async Task<IReadOnlyList<WordReview>> GetDueReviewsAsync(Guid userId, DateTime asOf, CancellationToken ct = default) =>
        await _context.WordReviews
            .Where(w => w.UserId == userId && w.NextReviewAt <= asOf)
            .OrderBy(w => w.NextReviewAt)
            .ToListAsync(ct);

    public async Task AddAsync(WordReview review, CancellationToken ct = default) =>
        await _context.WordReviews.AddAsync(review, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    public async Task<IReadOnlyList<Guid>> GetUserIdsWithOverdueReviewsAsync(DateTime overdueBefore, CancellationToken ct = default) =>
        await _context.WordReviews
            .Where(w => w.NextReviewAt <= overdueBefore)
            .Select(w => w.UserId)
            .Distinct()
            .ToListAsync(ct);
}