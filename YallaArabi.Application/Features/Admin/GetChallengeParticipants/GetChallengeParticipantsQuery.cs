using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetChallengeParticipants;

public sealed record GetChallengeParticipantsQuery(Guid ChallengeId) : IRequest<Result<ChallengeParticipantsResponse>>;

public sealed record ChallengeParticipantsResponse(
    Guid ChallengeId,
    string ChallengeTitle,
    int TotalQuestions,
    int ParticipantsCount,
    int CompletedCount,
    double AverageScorePercentage,
    IReadOnlyList<ParticipantItem> Participants
);

public sealed record ParticipantItem(
    Guid ParticipationId,
    Guid UserId,
    string FullName,
    string Email,
    string? LevelCode,
    int CorrectAnswers,
    int TotalQuestions,
    int ScorePercentage,
    int PointsEarned,
    string Status,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int Rank
);