using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class WordRepository : IWordRepository
{
    private readonly AppDbContext _context;

    public WordRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<Word>> SearchAsync(
        string normalizedQuery,
        IReadOnlyList<Guid>? allowedLevelIds,
        int maxResults,
        CancellationToken ct = default)
    {
        var query = _context.Words.AsQueryable();

        if (allowedLevelIds is not null)
        {
            query = query.Where(w => allowedLevelIds.Contains(w.LevelId));
        }

        var pattern = $"%{normalizedQuery}%";
        query = query.Where(w =>
            EF.Functions.Like(w.NormalizedArabic, pattern) ||
            EF.Functions.Like(w.NormalizedTransliteration, pattern) ||
            EF.Functions.Like(w.NormalizedEnglish, pattern));

        return await query
            .OrderBy(w => w.ArabicText)
            .Take(maxResults)
            .ToListAsync(ct);
    }

    public async Task<Word?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Words.FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<IReadOnlyList<Word>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        return await _context.Words.Where(w => idList.Contains(w.Id)).ToListAsync(ct);
    }

    public async Task<(IReadOnlyList<Word> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm, Guid? levelId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Words.AsQueryable();

        if (levelId.HasValue)
        {
            query = query.Where(w => w.LevelId == levelId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";
            query = query.Where(w =>
                EF.Functions.Like(w.ArabicText, pattern) ||
                EF.Functions.Like(w.Transliteration, pattern) ||
                EF.Functions.Like(w.EnglishMeaning, pattern));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(Word word, CancellationToken ct = default) =>
        await _context.Words.AddAsync(word, ct);

    public async Task RemoveAsync(Word word, CancellationToken ct = default)
    {
        _context.Words.Remove(word);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}