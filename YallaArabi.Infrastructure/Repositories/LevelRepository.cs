using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class LevelRepository : ILevelRepository
{
    private readonly AppDbContext _context;

    public LevelRepository(AppDbContext context) => _context = context;

    public async Task<Level?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Levels.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<Level?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        await _context.Levels.FirstOrDefaultAsync(l => l.Code == code, ct);

    public async Task<IReadOnlyList<Level>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Levels.OrderBy(l => l.OrderIndex).ToListAsync(ct);
}