using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Subscriptions.ProcessWebhook;

public sealed record ProcessWebhookCommand(
    string Payload,
    string Signature,
    Guid UserId,
    string PlanCode,
    string PaymentProviderReference
) : IRequest<Result<Unit>>;