using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateChallengeQuestion;

public sealed record UpdateChallengeQuestionCommand(
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
) : IRequest<Result<Unit>>;