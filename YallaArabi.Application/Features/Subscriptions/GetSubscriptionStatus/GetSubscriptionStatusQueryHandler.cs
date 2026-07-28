using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Subscriptions.GetSubscriptionStatus;

public sealed class GetSubscriptionStatusQueryHandler : IRequestHandler<GetSubscriptionStatusQuery, Result<SubscriptionStatusResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSubscriptionStatusQueryHandler(
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SubscriptionStatusResponse>> Handle(GetSubscriptionStatusQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<SubscriptionStatusResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<SubscriptionStatusResponse>.Failure("user_not_found", "User not found");
        }

        var activeSubscription = await _subscriptionRepository.GetActiveByUserIdAsync(currentUserId.Value, cancellationToken);

        if (activeSubscription is null || !activeSubscription.IsCurrentlyActive())
        {
            return Result<SubscriptionStatusResponse>.Success(new SubscriptionStatusResponse("free", null, null));
        }

        return Result<SubscriptionStatusResponse>.Success(new SubscriptionStatusResponse("premium", activeSubscription.PlanCode, activeSubscription.ExpiresAt));
    }
}