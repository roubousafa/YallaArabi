using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.PublishChallenge;

public sealed record PublishChallengeCommand(Guid ChallengeId) : IRequest<Result<Unit>>;