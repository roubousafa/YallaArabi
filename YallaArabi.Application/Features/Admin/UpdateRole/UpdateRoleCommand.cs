using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateRole;

public sealed record UpdateRoleCommand(
    Guid RoleId,
    string NameEn,
    string NameAr,
    string? Description,
    IReadOnlyList<string> Permissions
) : IRequest<Result<Unit>>;