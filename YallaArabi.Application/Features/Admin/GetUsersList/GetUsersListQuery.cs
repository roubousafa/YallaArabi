using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetUsersList;

public sealed record GetUsersListQuery(
    string? SearchTerm,
    string? Role,
    string? LevelCode,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedUsersResponse>>;

public sealed record PagedUsersResponse(
    IReadOnlyList<AdminUserItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public sealed record AdminUserItem(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    string? LevelCode,
    string SubscriptionStatus,
    int? DailyGoalMinutes,
    string? LearningMotivation,
    bool HasGoogleAccount,
    DateTime CreatedAt
);