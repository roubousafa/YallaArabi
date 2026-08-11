using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.CancelSubscription;

public sealed class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, Result<Unit>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
    }

    public async ValueTask<Result<Unit>> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId, cancellationToken);

        if (subscription is null)
        {
            return Result<Unit>.Failure("subscription_not_found", "Subscription not found");
        }

        if (subscription.Status == "cancelled")
        {
            return Result<Unit>.Failure("already_cancelled", "This subscription is already cancelled");
        }

        subscription.Cancel();
        await _subscriptionRepository.SaveChangesAsync(cancellationToken);

        var user = await _userRepository.GetByIdAsync(subscription.UserId, cancellationToken);

        if (user is not null)
        {
            var stillHasActive = await _subscriptionRepository.GetActiveByUserIdAsync(subscription.UserId, cancellationToken);

            if (stillHasActive is null || !stillHasActive.IsCurrentlyActive())
            {
                user.DeactivatePremium();
                await _userRepository.SaveChangesAsync(cancellationToken);
            }
        }

        return Result<Unit>.Success(Unit.Value);
    }
}