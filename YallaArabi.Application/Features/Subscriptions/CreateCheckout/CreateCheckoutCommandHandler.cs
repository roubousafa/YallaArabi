using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Subscriptions.CreateCheckout;

public sealed class CreateCheckoutCommandHandler : IRequestHandler<CreateCheckoutCommand, Result<CreateCheckoutResponse>>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly ICurrentUserService _currentUserService;

    public CreateCheckoutCommandHandler(IPaymentGateway paymentGateway, ICurrentUserService currentUserService)
    {
        _paymentGateway = paymentGateway;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<CreateCheckoutResponse>> Handle(CreateCheckoutCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<CreateCheckoutResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var session = await _paymentGateway.CreateCheckoutSessionAsync(currentUserId.Value, request.PlanCode, cancellationToken);

        return Result<CreateCheckoutResponse>.Success(new CreateCheckoutResponse(session.CheckoutUrl));
    }
}