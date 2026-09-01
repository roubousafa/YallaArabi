using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.AssignUserRole;

public sealed class AssignUserRoleCommandHandler : IRequestHandler<AssignUserRoleCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentUserService _currentUserService;

    public AssignUserRoleCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<Unit>> Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
    {
        var currentAdminId = _currentUserService.UserId;

        if (currentAdminId is null)
        {
            return Result<Unit>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        if (currentAdminId.Value == request.UserId)
        {
            return Result<Unit>.Failure("cannot_change_own_role", "You cannot change your own role");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<Unit>.Failure("user_not_found", "User not found");
        }

        if (request.RoleId is null)
        {
            user.RemoveRole();
            await _userRepository.SaveChangesAsync(cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId.Value, cancellationToken);

        if (role is null)
        {
            return Result<Unit>.Failure("role_not_found", "Role not found");
        }

        user.AssignRole(role.Id);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}