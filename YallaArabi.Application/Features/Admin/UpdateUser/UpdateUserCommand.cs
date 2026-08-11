using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string FullName,
    string Role,
    string? LevelCode
) : IRequest<Result<UpdateUserResponse>>;

public sealed record UpdateUserResponse(Guid UserId, string FullName, string Role, string? LevelCode);