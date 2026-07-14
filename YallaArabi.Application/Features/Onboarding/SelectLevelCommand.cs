using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Onboarding;

public sealed record SelectLevelCommand(string AssessmentKey) : IRequest<Result<SelectLevelResponse>>;

public sealed record SelectLevelResponse(string LevelCode, string LevelName);