using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IAiDictionaryRepository
{
    Task<AiDictionaryEntry?> GetAsync(string normalizedQuery, string dialect, CancellationToken ct = default);
    Task<AiDictionaryEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<int> CountRequestedByUserOnDateAsync(Guid userId, DateOnly localDate, CancellationToken ct = default);

    Task<(IReadOnlyList<AiDictionaryEntry> Items, int TotalCount)> GetPagedAsync(
        string? status, string? dialect, int pageNumber, int pageSize, CancellationToken ct = default);

    // Saves a new entry; when another request saved the same query and dialect first, returns that one instead.
    Task<AiDictionaryEntry> AddOrGetExistingAsync(AiDictionaryEntry entry, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IAiModeSettingsRepository
{
    // Returns the single settings row, creating it with defaults the first time.
    Task<AiModeSettings> GetAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
