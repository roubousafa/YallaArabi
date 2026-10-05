using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Notifications.MarkNotificationRead;

public sealed record MarkNotificationReadCommand(Guid NotificationId) : IRequest<Result<UnreadCountResponse>>;

public sealed record UnreadCountResponse(int UnreadCount);

public sealed class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Result<UnreadCountResponse>>
{
    private readonly IUserNotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public MarkNotificationReadCommandHandler(IUserNotificationRepository notificationRepository, ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<UnreadCountResponse>> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<UnreadCountResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);

        // Another student's notification is reported as not found so ids can't be probed.
        if (notification is null || notification.UserId != currentUserId.Value)
        {
            return Result<UnreadCountResponse>.Failure("notification_not_found", "Notification not found");
        }

        notification.MarkAsRead();
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        var unreadCount = await _notificationRepository.CountUnreadAsync(currentUserId.Value, cancellationToken);
        return Result<UnreadCountResponse>.Success(new UnreadCountResponse(unreadCount));
    }
}
