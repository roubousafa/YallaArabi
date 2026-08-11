using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IWordRepository
{
    Task<IReadOnlyList<Word>> SearchAsync(string normalizedQuery, IReadOnlyList<Guid>? allowedLevelIds, int maxResults, CancellationToken ct = default);
    Task<Word?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Word> Items, int TotalCount)> GetPagedAsync(string? searchTerm, Guid? levelId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task AddAsync(Word word, CancellationToken ct = default);
    Task RemoveAsync(Word word, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}