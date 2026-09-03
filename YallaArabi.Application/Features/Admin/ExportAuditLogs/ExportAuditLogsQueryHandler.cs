using System.Text;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.ExportAuditLogs;

public sealed class ExportAuditLogsQueryHandler : IRequestHandler<ExportAuditLogsQuery, Result<ExportAuditLogsResponse>>
{
    private const int MaxExportRecords = 10000;

    private readonly IAuditLogRepository _auditLogRepository;

    public ExportAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async ValueTask<Result<ExportAuditLogsResponse>> Handle(ExportAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _auditLogRepository.GetForExportAsync(
            request.UserId, request.FromDate, request.ToDate, MaxExportRecords, cancellationToken);

        var builder = new StringBuilder();
        builder.AppendLine("OccurredAt,UserFullName,UserEmail,ActionEn,ActionAr,EntityType,EntityId,HttpMethod,Path,StatusCode,IpAddress");

        foreach (var log in logs)
        {
            builder.AppendLine(string.Join(",",
                Escape(log.OccurredAt.ToString("yyyy-MM-dd HH:mm:ss")),
                Escape(log.UserFullName),
                Escape(log.UserEmail),
                Escape(log.ActionEn),
                Escape(log.ActionAr),
                Escape(log.EntityType),
                Escape(log.EntityId),
                Escape(log.HttpMethod),
                Escape(log.Path),
                log.StatusCode.ToString(),
                Escape(log.IpAddress)));
        }

        var utf8Bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var csvBytes = Encoding.UTF8.GetBytes(builder.ToString());
        var content = utf8Bom.Concat(csvBytes).ToArray();

        var fileName = $"audit-log-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";

        return Result<ExportAuditLogsResponse>.Success(
            new ExportAuditLogsResponse(content, fileName, logs.Count));
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}