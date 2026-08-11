using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteLevel;

public sealed record DeleteLevelCommand(Guid LevelId) : IRequest<Result<Unit>>;