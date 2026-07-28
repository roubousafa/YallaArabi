using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Subscriptions.CreateCheckout;

public sealed record CreateCheckoutCommand(string PlanCode) : IRequest<Result<CreateCheckoutResponse>>;

public sealed record CreateCheckoutResponse(string CheckoutUrl);