using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteChallenge;

public sealed record DeleteChallengeCommand(Guid ChallengeId) : IRequest<Result<Unit>>;