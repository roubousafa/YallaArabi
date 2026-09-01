using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateStudent;

public sealed class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;

    public UpdateStudentCommandHandler(IUserRepository userRepository, ILevelRepository levelRepository)
    {
        _userRepository = userRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<Unit>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
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

        Guid? levelId = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

            if (level is null)
            {
                return Result<Unit>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
        }

        user.UpdateByAdmin(request.FullName, levelId);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}