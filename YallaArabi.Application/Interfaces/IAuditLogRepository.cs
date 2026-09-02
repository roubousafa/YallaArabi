using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetPagedAsync(
        Guid? userId,
        string? action,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<IReadOnlyList<AuditLog>> GetForExportAsync(
        Guid? userId,
        DateTime? fromDate,
        DateTime? toDate,
        int maxRecords,
        CancellationToken ct = default);

    Task AddAsync(AuditLog log, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}