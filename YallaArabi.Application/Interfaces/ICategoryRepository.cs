using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetByLevelIdAsync(Guid levelId, CancellationToken ct = default);
}