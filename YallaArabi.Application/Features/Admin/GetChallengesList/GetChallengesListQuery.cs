using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetChallengesList;

public sealed record GetChallengesListQuery(
    string? Status,
    string? LevelCode,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedChallengesResponse>>;

public sealed record PagedChallengesResponse(
    IReadOnlyList<AdminChallengeItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public sealed record AdminChallengeItem(
    Guid ChallengeId,
    string Title,
    string? Description,
    string? LevelCode,
    int PointsReward,
    int TimeLimitSeconds,
    string Status,
    int QuestionsCount,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);