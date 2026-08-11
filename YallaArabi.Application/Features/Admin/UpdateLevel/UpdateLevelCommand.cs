using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateLevel;

public sealed record UpdateLevelCommand(Guid LevelId, string Name, int OrderIndex) : IRequest<Result<UpdateLevelResponse>>;

public sealed record UpdateLevelResponse(Guid LevelId, string Code, string Name, int OrderIndex);