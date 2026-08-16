using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Challenges.GetChallengeComparison;

public sealed record GetChallengeComparisonQuery(Guid ChallengeId, Guid OpponentUserId) : IRequest<Result<ChallengeComparisonResponse>>;

public sealed record ChallengeComparisonResponse(
    Guid ChallengeId,
    string ChallengeTitle,
    ParticipantResult Me,
    ParticipantResult? Opponent,
    string Outcome
);

public sealed record ParticipantResult(
    Guid UserId,
    string FullName,
    int CorrectAnswers,
    int TotalQuestions,
    int ScorePercentage,
    int PointsEarned,
    DateTime? CompletedAt
);