using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GrantSubscription;

public sealed record GrantSubscriptionCommand(
    Guid UserId,
    string PlanCode,
    int DurationDays
) : IRequest<Result<GrantSubscriptionResponse>>;

public sealed record GrantSubscriptionResponse(
    Guid SubscriptionId,
    Guid UserId,
    string PlanCode,
    DateTime ExpiresAt
);