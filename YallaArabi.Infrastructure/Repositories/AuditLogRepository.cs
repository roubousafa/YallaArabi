using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context) => _context = context;

    public async Task<(IReadOnlyList<AuditLog> Items, int TotalCount)> GetPagedAsync(
        Guid? userId,
        string? action,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = BuildQuery(userId, fromDate, toDate);

        if (!string.IsNullOrWhiteSpace(action))
        {
            var pattern = $"%{action.Trim()}%";
            query = query.Where(a => EF.Functions.Like(a.Action, pattern));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(a => a.OccurredAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<AuditLog>> GetForExportAsync(
        Guid? userId,
        DateTime? fromDate,
        DateTime? toDate,
        int maxRecords,
        CancellationToken ct = default)
    {
        return await BuildQuery(userId, fromDate, toDate)
            .OrderByDescending(a => a.OccurredAt)
            .Take(maxRecords)
            .ToListAsync(ct);
    }

    public async Task AddAsync(AuditLog log, CancellationToken ct = default) =>
        await _context.AuditLogs.AddAsync(log, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    private IQueryable<AuditLog> BuildQuery(Guid? userId, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.OccurredAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(a => a.OccurredAt <= toDate.Value);
        }

        return query;
    }
}