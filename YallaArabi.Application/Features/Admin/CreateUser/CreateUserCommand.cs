using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.CreateUser;

public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    string Role,
    string? LevelCode
) : IRequest<Result<CreateUserResponse>>;

public sealed record CreateUserResponse(Guid UserId, string FullName, string Email, string Role);