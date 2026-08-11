using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result<Unit>>;