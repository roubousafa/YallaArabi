using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Challenges.StartChallenge;

public sealed record StartChallengeCommand(Guid ChallengeId) : IRequest<Result<StartChallengeResponse>>;

public sealed record StartChallengeResponse(
    Guid ParticipationId,
    Guid ChallengeId,
    string Title,
    int TimeLimitSeconds,
    int PointsReward,
    DateTime StartedAt,
    IReadOnlyList<StudentQuestionItem> Questions
);

public sealed record StudentQuestionItem(
    Guid QuestionId,
    string QuestionText,
    string OptionA,
    string OptionB,
    string? OptionC,
    string? OptionD,
    int OrderIndex
);