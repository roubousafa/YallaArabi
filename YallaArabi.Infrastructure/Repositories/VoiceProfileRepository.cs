using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class VoiceProfileRepository : IVoiceProfileRepository
{
    private readonly AppDbContext _context;

    public VoiceProfileRepository(AppDbContext context) => _context = context;

    public async Task<VoiceProfile?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.VoiceProfiles.FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task<IReadOnlyList<VoiceProfile>> GetAllAsync(CancellationToken ct = default) =>
        await _context.VoiceProfiles
            .OrderBy(v => v.Dialect)
            .ThenBy(v => v.Gender)
            .ToListAsync(ct);

    public async Task<VoiceProfile?> FindAsync(string dialect, string gender, string source, CancellationToken ct = default) =>
        await _context.VoiceProfiles
            .FirstOrDefaultAsync(v =>
                v.Dialect == dialect &&
                v.Gender == gender &&
                v.Source == source &&
                v.IsActive, ct);

    public async Task AddAsync(VoiceProfile profile, CancellationToken ct = default) =>
        await _context.VoiceProfiles.AddAsync(profile, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}