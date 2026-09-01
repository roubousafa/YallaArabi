using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Auth.Login;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRoleRepository _roleRepository;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
                IRoleRepository roleRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _roleRepository = roleRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async ValueTask<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || user.PasswordHash is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure("invalid_credentials", "Invalid email or password");
        }

        if (user.MustChangePassword)
        {
            return Result<AuthResponse>.Failure(
                "password_change_required",
                "You must reset your password before signing in. Please check your email for the reset link.");
        }

        IReadOnlyList<string>? permissions = null;

        if (user.RoleId.HasValue)
        {
            permissions = await _roleRepository.GetPermissionKeysAsync(user.RoleId.Value, cancellationToken);
        }

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, permissions);
        var refreshTokenPlain = _jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = HashToken(refreshTokenPlain);

        var refreshTokenEntity = Domain.Entities.RefreshToken.Issue(user.Id, refreshTokenHash, DateTime.UtcNow.AddDays(30));
        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(user.Id, user.FullName, user.Email.Value, accessToken, refreshTokenPlain);
        return Result<AuthResponse>.Success(response);
    }

    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}