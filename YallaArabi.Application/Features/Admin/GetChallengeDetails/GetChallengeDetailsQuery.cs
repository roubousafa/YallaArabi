using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetChallengeDetails;

public sealed record GetChallengeDetailsQuery(Guid ChallengeId) : IRequest<Result<ChallengeDetailsResponse>>;

public sealed record ChallengeDetailsResponse(
    Guid ChallengeId,
    string Title,
    string? Description,
    string? LevelCode,
    int PointsReward,
    int TimeLimitSeconds,
    string Status,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    IReadOnlyList<ChallengeQuestionItem> Questions
);

public sealed record ChallengeQuestionItem(
    Guid QuestionId,
    string QuestionText,
    string OptionA,
    string OptionB,
    string? OptionC,
    string? OptionD,
    string CorrectOption,
    int OrderIndex,
    string? AudioUrl,
    string? ImageUrl
);