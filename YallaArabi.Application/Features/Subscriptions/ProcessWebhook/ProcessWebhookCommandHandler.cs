using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Subscriptions.ProcessWebhook;

public sealed class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, Result<Unit>>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;

    public ProcessWebhookCommandHandler(
        IPaymentGateway paymentGateway,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository)
    {
        _paymentGateway = paymentGateway;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
    }

    public async ValueTask<Result<Unit>> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
    {
        var isValidSignature = _paymentGateway.VerifyWebhookSignature(request.Payload, request.Signature);

        if (!isValidSignature)
        {
            return Result<Unit>.Failure("invalid_signature", "Webhook signature verification failed");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<Unit>.Failure("user_not_found", "User not found");
        }

        var expiresAt = request.PlanCode == "yearly"
            ? DateTime.UtcNow.AddYears(1)
            : DateTime.UtcNow.AddMonths(1);

        var subscription = Subscription.Create(request.UserId, request.PlanCode, request.PaymentProviderReference, expiresAt);
        await _subscriptionRepository.AddAsync(subscription, cancellationToken);

        user.ActivatePremium();
        await _userRepository.SaveChangesAsync(cancellationToken);
        await _subscriptionRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}