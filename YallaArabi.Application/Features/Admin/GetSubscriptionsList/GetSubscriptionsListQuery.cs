using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetSubscriptionsList;

public sealed record GetSubscriptionsListQuery(
    string? Status,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedSubscriptionsResponse>>;

public sealed record PagedSubscriptionsResponse(
    IReadOnlyList<AdminSubscriptionItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public sealed record AdminSubscriptionItem(
    Guid SubscriptionId,
    Guid UserId,
    string UserFullName,
    string UserEmail,
    string PlanCode,
    string Status,
    DateTime StartedAt,
    DateTime? ExpiresAt,
    bool IsCurrentlyActive,
    string? PaymentProviderReference
);