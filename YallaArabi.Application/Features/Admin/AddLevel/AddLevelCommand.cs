using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.AddLevel;

public sealed record AddLevelCommand(string Code, string Name, int OrderIndex) : IRequest<Result<AddLevelResponse>>;

public sealed record AddLevelResponse(Guid LevelId, string Code, string Name);