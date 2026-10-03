using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class RecordingSessionRepository : IRecordingSessionRepository
{
    private readonly AppDbContext _context;

    public RecordingSessionRepository(AppDbContext context) => _context = context;

    public async Task<RecordingSession?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.RecordingSessions.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<RecordingSession>> GetAllAsync(CancellationToken ct = default) =>
        await _context.RecordingSessions
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<RecordingSessionItem>> GetItemsAsync(Guid sessionId, CancellationToken ct = default) =>
        await _context.RecordingSessionItems
            .Where(i => i.SessionId == sessionId)
            .OrderBy(i => i.OrderNumber)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<RecordingSessionItem>> GetFlaggedItemsAsync(Guid sessionId, CancellationToken ct = default) =>
        await _context.RecordingSessionItems
            .Where(i => i.SessionId == sessionId && i.Status == "flagged")
            .OrderBy(i => i.OrderNumber)
            .ToListAsync(ct);

    public async Task<RecordingSessionItem?> GetItemByIdAsync(Guid itemId, CancellationToken ct = default) =>
        await _context.RecordingSessionItems.FirstOrDefaultAsync(i => i.Id == itemId, ct);

    public async Task AddAsync(RecordingSession session, CancellationToken ct = default) =>
        await _context.RecordingSessions.AddAsync(session, ct);

    public async Task AddItemsAsync(IEnumerable<RecordingSessionItem> items, CancellationToken ct = default) =>
        await _context.RecordingSessionItems.AddRangeAsync(items, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}