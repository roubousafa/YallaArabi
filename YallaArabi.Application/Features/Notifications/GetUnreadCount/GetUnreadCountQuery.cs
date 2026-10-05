using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Notifications.MarkNotificationRead;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Notifications.GetUnreadCount;

public sealed record GetUnreadCountQuery : IRequest<Result<UnreadCountResponse>>;

public sealed class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, Result<UnreadCountResponse>>
{
    private readonly IUserNotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetUnreadCountQueryHandler(IUserNotificationRepository notificationRepository, ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<UnreadCountResponse>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<UnreadCountResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var unreadCount = await _notificationRepository.CountUnreadAsync(currentUserId.Value, cancellationToken);
        return Result<UnreadCountResponse>.Success(new UnreadCountResponse(unreadCount));
    }
}
