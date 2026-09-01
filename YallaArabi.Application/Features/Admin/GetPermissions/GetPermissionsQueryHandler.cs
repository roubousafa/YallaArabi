using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Admin.GetPermissions;

public sealed class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<IReadOnlyList<PermissionGroupItem>>>
{
    public ValueTask<Result<IReadOnlyList<PermissionGroupItem>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var groups = Permissions.All
            .GroupBy(p => p.Group)
            .Select(g => new PermissionGroupItem(
                g.Key,
                g.Select(p => new PermissionItem(p.Key, p.NameAr, p.NameEn)).ToList()))
            .ToList();

        return ValueTask.FromResult(
            Result<IReadOnlyList<PermissionGroupItem>>.Success(groups));
    }
}