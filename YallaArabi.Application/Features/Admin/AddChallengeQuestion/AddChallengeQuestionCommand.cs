using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.AddChallengeQuestion;

public sealed record AddChallengeQuestionCommand(
    Guid ChallengeId,
    string QuestionText,
    string OptionA,
    string OptionB,
    string? OptionC,
    string? OptionD,
    string CorrectOption
) : IRequest<Result<AddChallengeQuestionResponse>>;

public sealed record AddChallengeQuestionResponse(
    Guid QuestionId,
    Guid ChallengeId,
    int OrderIndex,
    int TotalQuestions
);