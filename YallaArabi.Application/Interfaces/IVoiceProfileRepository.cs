using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IVoiceProfileRepository
{
    Task<VoiceProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<VoiceProfile>> GetAllAsync(CancellationToken ct = default);
    Task<VoiceProfile?> FindAsync(string dialect, string gender, string source, CancellationToken ct = default);
    Task AddAsync(VoiceProfile profile, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}