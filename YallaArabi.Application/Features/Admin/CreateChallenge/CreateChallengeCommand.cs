using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.CreateChallenge;

public sealed record CreateChallengeCommand(
    string Title,
    string? Description,
    string? LevelCode,
    int PointsReward,
    int TimeLimitSeconds,
    DateTime? ExpiresAt
) : IRequest<Result<CreateChallengeResponse>>;

public sealed record CreateChallengeResponse(
    Guid ChallengeId,
    string Title,
    string? LevelCode,
    string Status
);