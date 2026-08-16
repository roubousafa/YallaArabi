using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Challenges.GetAvailableChallenges;

public sealed record GetAvailableChallengesQuery : IRequest<Result<IReadOnlyList<AvailableChallengeItem>>>;

public sealed record AvailableChallengeItem(
    Guid ChallengeId,
    string Title,
    string? Description,
    int PointsReward,
    int TimeLimitSeconds,
    int QuestionsCount,
    bool AlreadyCompleted,
    int? MyScore,
    DateTime? ExpiresAt
);