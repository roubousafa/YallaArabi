using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class PointRuleRepository : IPointRuleRepository
{
    private readonly AppDbContext _context;

    public PointRuleRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<PointRule>> GetAllAsync(CancellationToken ct = default) =>
        await _context.PointRules.OrderBy(p => p.ActionKey).ToListAsync(ct);

    public async Task<PointRule?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.PointRules.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<PointRule?> GetByActionKeyAsync(string actionKey, CancellationToken ct = default) =>
        await _context.PointRules.FirstOrDefaultAsync(p => p.ActionKey == actionKey, ct);

    public async Task<bool> ActionKeyExistsAsync(string actionKey, CancellationToken ct = default) =>
        await _context.PointRules.AnyAsync(p => p.ActionKey == actionKey, ct);

    public async Task AddAsync(PointRule rule, CancellationToken ct = default) =>
        await _context.PointRules.AddAsync(rule, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}