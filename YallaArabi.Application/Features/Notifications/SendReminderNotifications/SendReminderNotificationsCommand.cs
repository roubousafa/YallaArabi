using Mediator;

namespace YallaArabi.Application.Features.Notifications.SendReminderNotifications;

public sealed record SendReminderNotificationsCommand : IRequest<SendReminderNotificationsResponse>;

public sealed record SendReminderNotificationsResponse(int NotificationsSent, int SkippedNoToken);