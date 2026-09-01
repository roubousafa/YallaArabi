using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Admin.UpdateRole;

public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<Unit>>
{
    private readonly IRoleRepository _roleRepository;

    public UpdateRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async ValueTask<Result<Unit>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result<Unit>.Failure("role_not_found", "Role not found");
        }

        if (role.IsSystemRole)
        {
            return Result<Unit>.Failure("system_role_locked", "System roles cannot be modified");
        }

        var nameEn = request.NameEn.Trim();
        var exists = await _roleRepository.NameExistsAsync(nameEn, role.Id, cancellationToken);

        if (exists)
        {
            return Result<Unit>.Failure("role_name_exists", $"A role named '{nameEn}' already exists");
        }

        var validKeys = Permissions.All.Select(p => p.Key).ToHashSet();
        var invalid = request.Permissions.Where(p => !validKeys.Contains(p)).ToList();

        if (invalid.Count > 0)
        {
            return Result<Unit>.Failure("invalid_permissions", $"Unknown permission keys: {string.Join(", ", invalid)}");
        }

        role.Update(nameEn, request.NameAr, request.Description);
        role.SetPermissions(request.Permissions);

        await _roleRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}