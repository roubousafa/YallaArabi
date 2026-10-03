using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IRecordingSessionRepository
{
    Task<RecordingSession?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<RecordingSession>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RecordingSessionItem>> GetItemsAsync(Guid sessionId, CancellationToken ct = default);
    Task<IReadOnlyList<RecordingSessionItem>> GetFlaggedItemsAsync(Guid sessionId, CancellationToken ct = default);
    Task<RecordingSessionItem?> GetItemByIdAsync(Guid itemId, CancellationToken ct = default);
    Task AddAsync(RecordingSession session, CancellationToken ct = default);
    Task AddItemsAsync(IEnumerable<RecordingSessionItem> items, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}