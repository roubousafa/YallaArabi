using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.GrantSubscription;

public sealed class GrantSubscriptionCommandHandler : IRequestHandler<GrantSubscriptionCommand, Result<GrantSubscriptionResponse>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;

    public GrantSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
    }

    public async ValueTask<Result<GrantSubscriptionResponse>> Handle(GrantSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<GrantSubscriptionResponse>.Failure("user_not_found", "User not found");
        }

        var existingActive = await _subscriptionRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);

        if (existingActive is not null && existingActive.IsCurrentlyActive())
        {
            existingActive.Cancel();
        }

        var expiresAt = DateTime.UtcNow.AddDays(request.DurationDays);
        var reference = $"admin_grant_{DateTime.UtcNow:yyyyMMddHHmmss}";

        var subscription = Subscription.Create(request.UserId, request.PlanCode, reference, expiresAt);

        await _subscriptionRepository.AddAsync(subscription, cancellationToken);

        user.ActivatePremium();

        await _subscriptionRepository.SaveChangesAsync(cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<GrantSubscriptionResponse>.Success(
            new GrantSubscriptionResponse(subscription.Id, request.UserId, subscription.PlanCode, expiresAt));
    }
}