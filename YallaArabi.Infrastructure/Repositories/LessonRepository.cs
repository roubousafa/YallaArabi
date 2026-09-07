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

    public async Task<int> CountByCategoryIdAsync(Guid categoryId, CancellationToken ct = default) =>
    await _context.Lessons.CountAsync(l => l.CategoryId == categoryId, ct);

    public async Task<int> CountCompletionsAsync(Guid lessonId, CancellationToken ct = default) =>
        await _context.StudentLessonProgress.CountAsync(p => p.LessonId == lessonId, ct);

    public async Task AddAsync(Lesson lesson, CancellationToken ct = default) =>
        await _context.Lessons.AddAsync(lesson, ct);

    public async Task RemoveAsync(Lesson lesson, CancellationToken ct = default)
    {
        _context.Lessons.Remove(lesson);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id, ct);
}