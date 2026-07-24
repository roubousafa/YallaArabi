using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async ValueTask<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
        {
            return Result<AuthResponse>.Failure("invalid_refresh_token", "Invalid or expired refresh token");
        }

        var user = await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken);

        if (user is null)
        {
            return Result<AuthResponse>.Failure("user_not_found", "User not found");
        }

        storedToken.Revoke();
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshTokenPlain = _jwtTokenGenerator.GenerateRefreshToken();
        var newRefreshTokenHash = HashToken(newRefreshTokenPlain);

        var newRefreshTokenEntity = Domain.Entities.RefreshToken.Issue(user.Id, newRefreshTokenHash, DateTime.UtcNow.AddDays(30));
        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        var response = new AuthResponse(user.Id, user.FullName, user.Email.Value, accessToken, newRefreshTokenPlain);
        return Result<AuthResponse>.Success(response);
    }

    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}