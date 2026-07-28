using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Subscriptions.GetSubscriptionStatus;

public sealed record GetSubscriptionStatusQuery : IRequest<Result<SubscriptionStatusResponse>>;

public sealed record SubscriptionStatusResponse(string SubscriptionStatus, string? PlanCode, DateTime? ExpiresAt);