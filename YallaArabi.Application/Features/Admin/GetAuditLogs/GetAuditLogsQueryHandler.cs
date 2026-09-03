using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, Result<PagedAuditLogsResponse>>
{
    private const int MaxPageSize = 100;

    private readonly IAuditLogRepository _auditLogRepository;

    public GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async ValueTask<Result<PagedAuditLogsResponse>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, MaxPageSize);

        var (items, totalCount) = await _auditLogRepository.GetPagedAsync(
            request.UserId,
            request.Action,
            request.FromDate,
            request.ToDate,
            pageNumber,
            pageSize,
            cancellationToken);

        var results = items
            .Select(a => new AuditLogItem(
                a.Id,
                a.UserId,
                a.UserFullName,
                a.UserEmail,
               a.ActionEn,
                a.ActionAr,
                a.EntityType,
                a.EntityId,
                a.HttpMethod,
                a.Path,
                a.StatusCode,
                a.IpAddress,
                a.OccurredAt))
            .ToList();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result<PagedAuditLogsResponse>.Success(
            new PagedAuditLogsResponse(results, totalCount, pageNumber, pageSize, totalPages));
    }
}