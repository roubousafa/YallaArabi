using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteChallengeQuestion;

public sealed record DeleteChallengeQuestionCommand(Guid QuestionId) : IRequest<Result<Unit>>;