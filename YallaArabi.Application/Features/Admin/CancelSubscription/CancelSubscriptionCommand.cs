using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.CancelSubscription;

public sealed record CancelSubscriptionCommand(Guid SubscriptionId) : IRequest<Result<Unit>>;