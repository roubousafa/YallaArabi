using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Notifications.MarkAllNotificationsRead;

public sealed record MarkAllNotificationsReadCommand : IRequest<Result<MarkAllNotificationsReadResponse>>;

public sealed record MarkAllNotificationsReadResponse(int MarkedCount);

public sealed class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, Result<MarkAllNotificationsReadResponse>>
{
    private readonly IUserNotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public MarkAllNotificationsReadCommandHandler(IUserNotificationRepository notificationRepository, ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<MarkAllNotificationsReadResponse>> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<MarkAllNotificationsReadResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var marked = await _notificationRepository.MarkAllAsReadAsync(currentUserId.Value, cancellationToken);
        return Result<MarkAllNotificationsReadResponse>.Success(new MarkAllNotificationsReadResponse(marked));
    }
}
