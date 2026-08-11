using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetNotificationsList;

public sealed record GetNotificationsListQuery(
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedNotificationsResponse>>;

public sealed record PagedNotificationsResponse(
    IReadOnlyList<AdminNotificationItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public sealed record AdminNotificationItem(
    Guid NotificationId,
    string Title,
    string Body,
    string TargetType,
    int RecipientsCount,
    int DeliveredCount,
    int SkippedCount,
    string SentByName,
    DateTime SentAt
);