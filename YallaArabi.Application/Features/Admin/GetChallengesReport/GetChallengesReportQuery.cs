using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetChallengesReport;

public sealed record GetChallengesReportQuery(
    Guid? UserId,
    string? LevelCode,
    Guid? ChallengeId,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedChallengesReportResponse>>;

public sealed record PagedChallengesReportResponse(
    IReadOnlyList<ChallengeReportItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    ChallengesReportSummary Summary
);

public sealed record ChallengeReportItem(
    Guid ParticipationId,
    Guid UserId,
    string StudentName,
    string StudentEmail,
    string? StudentLevelCode,
    Guid ChallengeId,
    string ChallengeTitle,
    string? ChallengeLevelCode,
    int CorrectAnswers,
    int TotalQuestions,
    int ScorePercentage,
    int PointsEarned,
    string Status,
    DateTime StartedAt,
    DateTime? CompletedAt
);

public sealed record ChallengesReportSummary(
    int TotalParticipations,
    int CompletedCount,
    int InProgressCount,
    double AverageScorePercentage
);