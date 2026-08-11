using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteUser;

public sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var currentAdminId = _currentUserService.UserId;

        if (currentAdminId == request.UserId)
        {
            return Result<Unit>.Failure("cannot_delete_self", "You cannot delete your own account");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<Unit>.Failure("user_not_found", "User not found");
        }

        await _userRepository.RemoveAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}