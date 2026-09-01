using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetPermissions;

public sealed record GetPermissionsQuery : IRequest<Result<IReadOnlyList<PermissionGroupItem>>>;

public sealed record PermissionGroupItem(
    string Group,
    IReadOnlyList<PermissionItem> Permissions
);

public sealed record PermissionItem(
    string Key,
    string NameAr,
    string NameEn
);