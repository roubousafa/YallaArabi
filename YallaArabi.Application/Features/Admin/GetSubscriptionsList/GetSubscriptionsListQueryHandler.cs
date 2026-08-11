using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetSubscriptionsList;

public sealed class GetSubscriptionsListQueryHandler : IRequestHandler<GetSubscriptionsListQuery, Result<PagedSubscriptionsResponse>>
{
    private const int MaxPageSize = 100;

    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;

    public GetSubscriptionsListQueryHandler(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
    }

    public async ValueTask<Result<PagedSubscriptionsResponse>> Handle(GetSubscriptionsListQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, MaxPageSize);

        var (items, totalCount) = await _subscriptionRepository.GetPagedAsync(
            request.Status, pageNumber, pageSize, cancellationToken);

        var results = new List<AdminSubscriptionItem>();

        foreach (var subscription in items)
        {
            var user = await _userRepository.GetByIdAsync(subscription.UserId, cancellationToken);

            results.Add(new AdminSubscriptionItem(
                subscription.Id,
                subscription.UserId,
                user?.FullName ?? "(deleted user)",
                user?.Email.Value ?? "-",
                subscription.PlanCode,
                subscription.Status,
                subscription.StartedAt,
                subscription.ExpiresAt,
                subscription.IsCurrentlyActive(),
                subscription.PaymentProviderReference));
        }

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result<PagedSubscriptionsResponse>.Success(
            new PagedSubscriptionsResponse(results, totalCount, pageNumber, pageSize, totalPages));
    }
}