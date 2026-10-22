using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.CreateStudent;

public sealed class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<CreateStudentResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateStudentCommandHandler(
        IUserRepository userRepository,
        ILevelRepository levelRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _levelRepository = levelRepository;
        _passwordHasher = passwordHasher;
    }

    public async ValueTask<Result<CreateStudentResponse>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);

        if (emailExists)
        {
            return Result<CreateStudentResponse>.Failure("email_already_exists", "This email is already registered");
        }

        string? levelCode = null;

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Register(request.FullName, request.Email, passwordHash);
        user.MarkEmailVerified();

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

            if (level is null)
            {
                return Result<CreateStudentResponse>.Failure("level_not_found", "Level not found");
            }

            user.SetCurrentLevel(level.Id);
            levelCode = level.Code;
        }

        await _userRepository.AddAsync(user, cancellationToken);

        return Result<CreateStudentResponse>.Success(
            new CreateStudentResponse(user.Id, user.FullName, user.Email.Value, levelCode));
    }
}