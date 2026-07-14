using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct = default);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}