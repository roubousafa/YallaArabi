using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteRole;

public sealed record DeleteRoleCommand(Guid RoleId) : IRequest<Result<Unit>>;