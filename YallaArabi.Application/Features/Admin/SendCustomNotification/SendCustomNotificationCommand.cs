using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.SendCustomNotification;

public sealed record SendCustomNotificationCommand(
    string Title,
    string Body,
    IReadOnlyList<Guid>? UserIds
) : IRequest<Result<SendCustomNotificationResponse>>;

public sealed record SendCustomNotificationResponse(
    Guid NotificationId,
    int RecipientsCount,
    int DeliveredCount,
    int SkippedCount
);