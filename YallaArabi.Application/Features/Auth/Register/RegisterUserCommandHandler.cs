using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Auth.Register;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async ValueTask<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return Result<AuthResponse>.Failure("email_already_exists", "This email is already registered");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var user = User.Register(request.FullName, request.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshTokenPlain = _jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = HashToken(refreshTokenPlain);

        var refreshTokenEntity = Domain.Entities.RefreshToken.Issue(user.Id, refreshTokenHash, DateTime.UtcNow.AddDays(30));
        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(user.Id, user.FullName, request.Email, accessToken, refreshTokenPlain);
        return Result<AuthResponse>.Success(response);
    }

    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}