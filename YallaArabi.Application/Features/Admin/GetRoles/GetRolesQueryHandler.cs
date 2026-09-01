using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Admin.GetRoles;

public sealed class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<IReadOnlyList<RoleItem>>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesQueryHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async ValueTask<Result<IReadOnlyList<RoleItem>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        var results = new List<RoleItem>();

        foreach (var role in roles)
        {
            var usersCount = await _roleRepository.CountUsersInRoleAsync(role.Id, cancellationToken);

            var permissionKeys = role.IsSystemRole
                ? Permissions.All.Select(p => p.Key).ToList()
                : role.Permissions.Select(p => p.PermissionKey).ToList();

            results.Add(new RoleItem(
                role.Id,
                role.NameEn,
                role.NameAr,
                role.Description,
                role.IsSystemRole,
                usersCount,
                permissionKeys.Count,
                permissionKeys,
                !role.IsSystemRole,
                !role.IsSystemRole && usersCount == 0,
                role.CreatedAt));
        }

        return Result<IReadOnlyList<RoleItem>>.Success(results);
    }
}