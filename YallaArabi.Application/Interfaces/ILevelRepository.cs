using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface ILevelRepository
{
    Task<Level?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Level?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<Level>> GetAllAsync(CancellationToken ct = default);
}