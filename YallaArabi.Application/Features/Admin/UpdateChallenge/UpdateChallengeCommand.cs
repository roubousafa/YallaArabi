using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateChallenge;

public sealed record UpdateChallengeCommand(
    Guid ChallengeId,
    string Title,
    string? Description,
    string? LevelCode,
    int PointsReward,
    int TimeLimitSeconds,
    DateTime? ExpiresAt
) : IRequest<Result<Unit>>;