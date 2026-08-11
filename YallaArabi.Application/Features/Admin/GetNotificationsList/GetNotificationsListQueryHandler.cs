using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetNotificationsList;

public sealed class GetNotificationsListQueryHandler : IRequestHandler<GetNotificationsListQuery, Result<PagedNotificationsResponse>>
{
    private const int MaxPageSize = 100;

    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;

    public GetNotificationsListQueryHandler(
        INotificationRepository notificationRepository,
        IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    public async ValueTask<Result<PagedNotificationsResponse>> Handle(GetNotificationsListQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, MaxPageSize);

        var (items, totalCount) = await _notificationRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);

        var results = new List<AdminNotificationItem>();

        foreach (var notification in items)
        {
            var sender = await _userRepository.GetByIdAsync(notification.SentByUserId, cancellationToken);

            results.Add(new AdminNotificationItem(
                notification.Id,
                notification.Title,
                notification.Body,
                notification.TargetType,
                notification.RecipientsCount,
                notification.DeliveredCount,
                notification.SkippedCount,
                sender?.FullName ?? "(deleted admin)",
                notification.SentAt));
        }

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result<PagedNotificationsResponse>.Success(
            new PagedNotificationsResponse(results, totalCount, pageNumber, pageSize, totalPages));
    }
}