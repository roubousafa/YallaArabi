using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.CreateUser;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        ILevelRepository levelRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _levelRepository = levelRepository;
        _passwordHasher = passwordHasher;
    }

    public async ValueTask<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);

        if (emailExists)
        {
            return Result<CreateUserResponse>.Failure("email_already_exists", "This email is already registered");
        }

        Guid? levelId = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

            if (level is null)
            {
                return Result<CreateUserResponse>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Register(request.FullName, request.Email, passwordHash);

        if (request.Role == "Admin")
        {
            user.PromoteToAdmin();
        }

        if (levelId.HasValue)
        {
            user.SetCurrentLevel(levelId.Value);
        }

        await _userRepository.AddAsync(user, cancellationToken);

        return Result<CreateUserResponse>.Success(
            new CreateUserResponse(user.Id, user.FullName, user.Email.Value, user.Role));
    }
}