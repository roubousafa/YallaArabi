using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.CreateTeamMember;

public sealed class CreateTeamMemberCommandHandler : IRequestHandler<CreateTeamMemberCommand, Result<CreateTeamMemberResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateTeamMemberCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    public async ValueTask<Result<CreateTeamMemberResponse>> Handle(CreateTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);

        if (emailExists)
        {
            return Result<CreateTeamMemberResponse>.Failure("email_already_exists", "This email is already registered");
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

        if (role is null)
        {
            return Result<CreateTeamMemberResponse>.Failure("role_not_found", "Role not found");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Register(request.FullName, request.Email, passwordHash);

        user.AssignRole(role.Id);

        await _userRepository.AddAsync(user, cancellationToken);

        return Result<CreateTeamMemberResponse>.Success(
            new CreateTeamMemberResponse(user.Id, user.FullName, user.Email.Value, role.Id, role.NameEn));
    }
}