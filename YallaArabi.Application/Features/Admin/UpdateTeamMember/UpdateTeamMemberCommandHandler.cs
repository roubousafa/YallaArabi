using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateTeamMember;

public sealed class UpdateTeamMemberCommandHandler : IRequestHandler<UpdateTeamMemberCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTeamMemberCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<Unit>> Handle(UpdateTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var currentAdminId = _currentUserService.UserId;

        if (currentAdminId is null)
        {
            return Result<Unit>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<Unit>.Failure("user_not_found", "Team member not found");
        }

        if (user.Role != "Admin")
        {
            return Result<Unit>.Failure("not_a_team_member", "This user is not a team member");
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result<Unit>.Failure("role_not_found", "Role not found");
        }

        if (currentAdminId.Value == request.UserId && user.RoleId != request.RoleId)
        {
            return Result<Unit>.Failure("cannot_change_own_role", "You cannot change your own role");
        }

        user.UpdateByAdmin(request.FullName, user.CurrentLevelId);
        user.AssignRole(role.Id);

        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}