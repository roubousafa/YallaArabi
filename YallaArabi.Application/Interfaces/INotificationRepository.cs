using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface INotificationRepository
{
    Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}