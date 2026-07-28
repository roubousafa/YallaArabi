using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class SavedWordRepository : ISavedWordRepository
{
    private readonly AppDbContext _context;

    public SavedWordRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<SavedWord>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.SavedWords
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SavedAt)
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid userId, Guid wordId, CancellationToken ct = default) =>
        await _context.SavedWords.AnyAsync(s => s.UserId == userId && s.WordId == wordId, ct);

    public async Task<SavedWord?> GetAsync(Guid userId, Guid wordId, CancellationToken ct = default) =>
        await _context.SavedWords.FirstOrDefaultAsync(s => s.UserId == userId && s.WordId == wordId, ct);

    public async Task AddAsync(SavedWord savedWord, CancellationToken ct = default) =>
        await _context.SavedWords.AddAsync(savedWord, ct);

    public async Task RemoveAsync(SavedWord savedWord, CancellationToken ct = default)
    {
        _context.SavedWords.Remove(savedWord);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}