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
}