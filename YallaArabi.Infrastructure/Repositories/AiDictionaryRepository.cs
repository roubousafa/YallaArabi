using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class AiDictionaryRepository : IAiDictionaryRepository
{
    private readonly AppDbContext _context;

    public AiDictionaryRepository(AppDbContext context) => _context = context;

    public async Task<AiDictionaryEntry?> GetAsync(string normalizedQuery, string dialect, CancellationToken ct = default) =>
        await _context.AiDictionaryEntries.FirstOrDefaultAsync(e => e.NormalizedQuery == normalizedQuery && e.Dialect == dialect, ct);

    public async Task<AiDictionaryEntry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.AiDictionaryEntries.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<int> CountRequestedByUserOnDateAsync(Guid userId, DateOnly localDate, CancellationToken ct = default) =>
        await _context.AiDictionaryEntries.CountAsync(e => e.RequestedByUserId == userId && e.RequestedLocalDate == localDate, ct);

    public async Task<(IReadOnlyList<AiDictionaryEntry> Items, int TotalCount)> GetPagedAsync(
        string? status, string? dialect, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.AiDictionaryEntries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(e => e.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(dialect))
        {
            query = query.Where(e => e.Dialect == dialect);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<AiDictionaryEntry> AddOrGetExistingAsync(AiDictionaryEntry entry, CancellationToken ct = default)
    {
        await _context.AiDictionaryEntries.AddAsync(entry, ct);

        try
        {
            await _context.SaveChangesAsync(ct);
            return entry;
        }
        catch (DbUpdateException)
        {
            // Two students searched the same new word at once; the unique index kept the first answer.
            _context.Entry(entry).State = EntityState.Detached;
            var existing = await GetAsync(entry.NormalizedQuery, entry.Dialect, ct);
            return existing ?? throw new InvalidOperationException("Failed to save the AI dictionary entry");
        }
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}

public class AiModeSettingsRepository : IAiModeSettingsRepository
{
    private readonly AppDbContext _context;

    public AiModeSettingsRepository(AppDbContext context) => _context = context;

    public async Task<AiModeSettings> GetAsync(CancellationToken ct = default)
    {
        var settings = await _context.AiModeSettings.FirstOrDefaultAsync(s => s.Id == AiModeSettings.SingletonId, ct);

        if (settings is not null)
        {
            return settings;
        }

        settings = AiModeSettings.CreateDefault();
        await _context.AiModeSettings.AddAsync(settings, ct);
        await _context.SaveChangesAsync(ct);
        return settings;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}
