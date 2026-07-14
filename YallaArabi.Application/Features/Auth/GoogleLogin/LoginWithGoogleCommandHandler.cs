using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Auth.GoogleLogin;

public sealed class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, Result<AuthResponse>>
{
    private readonly IGoogleAuthValidator _googleAuthValidator;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginWithGoogleCommandHandler(
        IGoogleAuthValidator googleAuthValidator,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _googleAuthValidator = googleAuthValidator;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async ValueTask<Result<AuthResponse>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        var googleUser = await _googleAuthValidator.ValidateAsync(request.IdToken, cancellationToken);

        if (googleUser is null)
        {
            return Result<AuthResponse>.Failure("invalid_google_token", "Invalid or expired Google token");
        }

        var user = await _userRepository.GetByGoogleIdAsync(googleUser.GoogleId, cancellationToken);

        if (user is null)
        {
            user = await _userRepository.GetByEmailAsync(googleUser.Email, cancellationToken);

            if (user is not null)
            {
                user.LinkGoogleAccount(googleUser.GoogleId);
                await _userRepository.SaveChangesAsync(cancellationToken);
            }
            else
            {
                user = User.RegisterWithGoogle(googleUser.FullName, googleUser.Email, googleUser.GoogleId);
                await _userRepository.AddAsync(user, cancellationToken);
            }
        }

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var refreshTokenPlain = _jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = HashToken(refreshTokenPlain);

        var refreshTokenEntity = RefreshToken.Issue(user.Id, refreshTokenHash, DateTime.UtcNow.AddDays(30));
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