using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class PointTransactionRepository : IPointTransactionRepository
{
    private readonly AppDbContext _context;

    public PointTransactionRepository(AppDbContext context) => _context = context;

    public async Task<int> GetTotalPointsAsync(Guid userId, CancellationToken ct = default) =>
        await _context.PointTransactions
            .Where(p => p.UserId == userId)
            .SumAsync(p => p.Points, ct);

    public async Task<IReadOnlyList<PointTransaction>> GetByUserIdAsync(Guid userId, int maxResults, CancellationToken ct = default) =>
        await _context.PointTransactions
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.EarnedAt)
            .Take(maxResults)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid userId, string actionKey, Guid relatedEntityId, CancellationToken ct = default) =>
        await _context.PointTransactions
            .AnyAsync(p => p.UserId == userId && p.ActionKey == actionKey && p.RelatedEntityId == relatedEntityId, ct);

    public async Task AddAsync(PointTransaction transaction, CancellationToken ct = default) =>
        await _context.PointTransactions.AddAsync(transaction, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}