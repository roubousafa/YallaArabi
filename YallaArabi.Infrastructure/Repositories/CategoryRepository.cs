using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Category>> GetByLevelIdAsync(Guid levelId, CancellationToken ct = default) =>
        await _context.Categories
            .Where(c => c.LevelId == levelId)
            .OrderBy(c => c.OrderIndex)
            .ToListAsync(ct);

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<int> CountByLevelIdAsync(Guid levelId, CancellationToken ct = default) =>
        await _context.Categories.CountAsync(c => c.LevelId == levelId, ct);

    public async Task<int> CountLessonsAsync(Guid categoryId, CancellationToken ct = default) =>
        await _context.Lessons.CountAsync(l => l.CategoryId == categoryId, ct);

    public async Task AddAsync(Category category, CancellationToken ct = default) =>
        await _context.Categories.AddAsync(category, ct);

    public async Task RemoveAsync(Category category, CancellationToken ct = default)
    {
        _context.Categories.Remove(category);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}