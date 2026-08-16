using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Challenges.SubmitChallenge;

public sealed record SubmitChallengeCommand(
    Guid ParticipationId,
    IReadOnlyList<SubmittedAnswer> Answers
) : IRequest<Result<SubmitChallengeResponse>>;

public sealed record SubmittedAnswer(Guid QuestionId, string SelectedOption);

public sealed record SubmitChallengeResponse(
    Guid ParticipationId,
    int TotalQuestions,
    int CorrectAnswers,
    int ScorePercentage,
    int PointsEarned,
    IReadOnlyList<AnswerResult> Results
);

public sealed record AnswerResult(
    Guid QuestionId,
    string QuestionText,
    string SelectedOption,
    string CorrectOption,
    bool IsCorrect
);