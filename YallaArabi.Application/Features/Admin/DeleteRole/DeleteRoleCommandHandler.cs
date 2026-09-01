using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteRole;

public sealed class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<Unit>>
{
    private readonly IRoleRepository _roleRepository;

    public DeleteRoleCommandHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result<Unit>.Failure("role_not_found", "Role not found");
        }

        if (role.IsSystemRole)
        {
            return Result<Unit>.Failure("system_role_locked", "System roles cannot be deleted");
        }

        var usersCount = await _roleRepository.CountUsersInRoleAsync(role.Id, cancellationToken);

        if (usersCount > 0)
        {
            return Result<Unit>.Failure(
                "role_in_use",
                $"Cannot delete this role because {usersCount} user(s) are assigned to it. Reassign them first.");
        }

        await _roleRepository.RemoveAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}