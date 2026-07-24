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
}