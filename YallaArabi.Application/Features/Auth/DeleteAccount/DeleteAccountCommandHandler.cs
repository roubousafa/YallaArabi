using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Auth.DeleteAccount;

public sealed class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountDeletionService _deletionService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAccountCommandHandler(
        IUserRepository userRepository,
        IAccountDeletionService deletionService,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _deletionService = deletionService;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<Unit>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<Unit>.Failure("user_not_found", "User not found");
        }

        if (user.Role == "Admin")
        {
            return Result<Unit>.Failure(
                "admin_cannot_self_delete",
                "Admin accounts cannot be deleted from the app. Please contact support.");
        }

        if (user.PasswordHash is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Result<Unit>.Failure("password_required", "Please enter your password to confirm account deletion");
            }

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Result<Unit>.Failure("invalid_password", "Incorrect password");
            }
        }

        await _deletionService.DeleteUserDataAsync(user.Id, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}