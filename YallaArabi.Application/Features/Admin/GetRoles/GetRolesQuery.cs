using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<IReadOnlyList<RoleItem>>>;

public sealed record RoleItem(
    Guid RoleId,
    string NameEn,
    string NameAr,
    string? Description,
    bool IsSystemRole,
    int UsersCount,
    int PermissionsCount,
    IReadOnlyList<string> Permissions,
    bool CanBeEdited,
    bool CanBeDeleted,
    DateTime CreatedAt
);