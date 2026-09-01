using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteStudent;

public sealed class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;

    public DeleteStudentCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<Unit>.Failure("user_not_found", "Student not found");
        }

        if (user.Role != "Student")
        {
            return Result<Unit>.Failure("not_a_student", "This user is not a student");
        }

        await _userRepository.RemoveAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}