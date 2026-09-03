using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    Guid? UserId,
    string? Action,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedAuditLogsResponse>>;

public sealed record PagedAuditLogsResponse(
    IReadOnlyList<AuditLogItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public sealed record AuditLogItem(
    Guid LogId,
    Guid UserId,
    string UserFullName,
    string UserEmail,
     string ActionEn,
    string ActionAr,
    string? EntityType,
    string? EntityId,
    string HttpMethod,
    string Path,
    int StatusCode,
    string? IpAddress,
    DateTime OccurredAt
);