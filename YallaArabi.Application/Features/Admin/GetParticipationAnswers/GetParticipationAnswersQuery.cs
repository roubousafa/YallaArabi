using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetParticipationAnswers;

public sealed record GetParticipationAnswersQuery(Guid ParticipationId) : IRequest<Result<ParticipationAnswersResponse>>;

public sealed record ParticipationAnswersResponse(
    Guid ParticipationId,
    Guid UserId,
    string StudentName,
    string StudentEmail,
    Guid ChallengeId,
    string ChallengeTitle,
    int CorrectAnswers,
    int TotalQuestions,
    int ScorePercentage,
    int PointsEarned,
    string Status,
    DateTime StartedAt,
    DateTime? CompletedAt,
    IReadOnlyList<StudentAnswerItem> Answers
);

public sealed record StudentAnswerItem(
    Guid QuestionId,
    string QuestionText,
    string? OptionA,
    string? OptionB,
    string? OptionC,
    string? OptionD,
    string? SelectedOption,
    string CorrectOption,
    bool IsCorrect,
    int OrderIndex
);