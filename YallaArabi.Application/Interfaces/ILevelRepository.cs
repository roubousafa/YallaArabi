using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface ILevelRepository
{
    Task<Level?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Level?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<Level>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<LevelContentCounts> GetContentCountsAsync(Guid levelId, CancellationToken ct = default);
    Task AddAsync(Level level, CancellationToken ct = default);
    Task RemoveAsync(Level level, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public sealed record LevelContentCounts(int CategoriesCount, int LessonsCount, int WordsCount, int StudentsCount);