using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.ExportAuditLogs;

public sealed record ExportAuditLogsQuery(
    Guid? UserId,
    DateTime? FromDate,
    DateTime? ToDate
) : IRequest<Result<ExportAuditLogsResponse>>;

public sealed record ExportAuditLogsResponse(byte[] Content, string FileName, int RecordsCount);