using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteTeamMember;

public sealed class DeleteTeamMemberCommandHandler : IRequestHandler<DeleteTeamMemberCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTeamMemberCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var currentAdminId = _currentUserService.UserId;

        if (currentAdminId == request.UserId)
        {
            return Result<Unit>.Failure("cannot_delete_self", "You cannot delete your own account");
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

        if (user.RoleId.HasValue)
        {
            var role = await _roleRepository.GetByIdAsync(user.RoleId.Value, cancellationToken);

            if (role is not null && role.IsSystemRole)
            {
                return Result<Unit>.Failure(
                    "cannot_delete_super_admin",
                    "Super Admin accounts cannot be deleted");
            }
        }

        await _userRepository.RemoveAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}