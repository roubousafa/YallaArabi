using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Lesson>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default) =>
        await _context.Lessons
            .Where(l => l.CategoryId == categoryId)
            .OrderBy(l => l.OrderIndex)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Lesson>> GetByCategoryIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken ct = default) =>
        await _context.Lessons
            .Where(l => categoryIds.Contains(l.CategoryId))
            .OrderBy(l => l.OrderIndex)
            .ToListAsync(ct);

    public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id, ct);
}