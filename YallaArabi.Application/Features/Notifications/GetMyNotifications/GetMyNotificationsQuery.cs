using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Notifications.GetMyNotifications;

public sealed record GetMyNotificationsQuery(int PageNumber = 1, int PageSize = 20) : IRequest<Result<MyNotificationsResponse>>;

public sealed record MyNotificationsResponse(
    IReadOnlyList<MyNotificationItem> Items,
    int UnreadCount,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public sealed record MyNotificationItem(
    Guid Id,
    string Type,
    string Title,
    string Body,
    Guid? RelatedEntityId,
    DateTime SentAt,
    bool IsRead
);

public sealed class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<MyNotificationsResponse>>
{
    private const int MaxPageSize = 50;

    private readonly IUserNotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyNotificationsQueryHandler(IUserNotificationRepository notificationRepository, ICurrentUserService currentUserService)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<MyNotificationsResponse>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<MyNotificationsResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var (items, totalCount) = await _notificationRepository.GetPagedAsync(currentUserId.Value, pageNumber, pageSize, cancellationToken);
        var unreadCount = await _notificationRepository.CountUnreadAsync(currentUserId.Value, cancellationToken);

        var response = new MyNotificationsResponse(
            items.Select(n => new MyNotificationItem(n.Id, n.Type, n.Title, n.Body, n.RelatedEntityId, n.CreatedAt, n.IsRead)).ToList(),
            unreadCount,
            totalCount,
            pageNumber,
            pageSize,
            (int)Math.Ceiling(totalCount / (double)pageSize));

        return Result<MyNotificationsResponse>.Success(response);
    }
}
