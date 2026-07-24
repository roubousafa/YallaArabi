using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Onboarding;

public sealed record SetLearningMotivationCommand(string MotivationKey) : IRequest<Result<SetLearningMotivationResponse>>;

public sealed record SetLearningMotivationResponse(string MotivationKey, string Title);