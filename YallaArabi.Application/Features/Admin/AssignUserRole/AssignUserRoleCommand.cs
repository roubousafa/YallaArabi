using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.AssignUserRole;

public sealed record AssignUserRoleCommand(Guid UserId, Guid? RoleId) : IRequest<Result<Unit>>;