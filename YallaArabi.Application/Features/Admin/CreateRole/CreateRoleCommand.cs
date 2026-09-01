using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.CreateRole;

public sealed record CreateRoleCommand(
    string NameEn,
    string NameAr,
    string? Description,
    IReadOnlyList<string> Permissions
) : IRequest<Result<CreateRoleResponse>>;

public sealed record CreateRoleResponse(Guid RoleId, string NameEn, string NameAr, int PermissionsCount);