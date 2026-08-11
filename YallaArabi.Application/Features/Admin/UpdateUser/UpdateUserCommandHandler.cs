using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateUser;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UpdateUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        ILevelRepository levelRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _levelRepository = levelRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<UpdateUserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UpdateUserResponse>.Failure("user_not_found", "User not found");
        }

        var currentAdminId = _currentUserService.UserId;

        if (currentAdminId == request.UserId && user.Role == "Admin" && request.Role != "Admin")
        {
            return Result<UpdateUserResponse>.Failure(
                "cannot_demote_self",
                "You cannot remove your own admin role");
        }

        Guid? levelId = null;
        string? levelCode = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

            if (level is null)
            {
                return Result<UpdateUserResponse>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
            levelCode = level.Code;
        }

        user.UpdateByAdmin(request.FullName, levelId);
        user.ChangeRole(request.Role);

        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<UpdateUserResponse>.Success(
            new UpdateUserResponse(user.Id, user.FullName, user.Role, levelCode));
    }
}