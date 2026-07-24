using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class StudentLessonProgressRepository : IStudentLessonProgressRepository
{
    private readonly AppDbContext _context;

    public StudentLessonProgressRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<StudentLessonProgress>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.StudentLessonProgress
            .Where(p => p.UserId == userId)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid userId, Guid lessonId, CancellationToken ct = default) =>
        await _context.StudentLessonProgress
            .AnyAsync(p => p.UserId == userId && p.LessonId == lessonId, ct);

    public async Task AddAsync(StudentLessonProgress progress, CancellationToken ct = default) =>
        await _context.StudentLessonProgress.AddAsync(progress, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}