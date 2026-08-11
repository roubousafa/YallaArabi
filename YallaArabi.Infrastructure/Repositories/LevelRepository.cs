using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class LevelRepository : ILevelRepository
{
    private readonly AppDbContext _context;

    public LevelRepository(AppDbContext context) => _context = context;

    public async Task<Level?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Levels.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<Level?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        await _context.Levels.FirstOrDefaultAsync(l => l.Code == code, ct);

    public async Task<IReadOnlyList<Level>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Levels.OrderBy(l => l.OrderIndex).ToListAsync(ct);

    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default) =>
        await _context.Levels.AnyAsync(l => l.Code == code, ct);

    public async Task<LevelContentCounts> GetContentCountsAsync(Guid levelId, CancellationToken ct = default)
    {
        var categoriesCount = await _context.Categories.CountAsync(c => c.LevelId == levelId, ct);

        var categoryIds = await _context.Categories
            .Where(c => c.LevelId == levelId)
            .Select(c => c.Id)
            .ToListAsync(ct);

        var lessonsCount = categoryIds.Count == 0
            ? 0
            : await _context.Lessons.CountAsync(l => categoryIds.Contains(l.CategoryId), ct);

        var wordsCount = await _context.Words.CountAsync(w => w.LevelId == levelId, ct);

        var studentsCount = await _context.Users.CountAsync(u => u.CurrentLevelId == levelId, ct);

        return new LevelContentCounts(categoriesCount, lessonsCount, wordsCount, studentsCount);
    }

    public async Task AddAsync(Level level, CancellationToken ct = default) =>
        await _context.Levels.AddAsync(level, ct);

    public async Task RemoveAsync(Level level, CancellationToken ct = default)
    {
        _context.Levels.Remove(level);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}