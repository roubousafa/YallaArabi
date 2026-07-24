using Mediator;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Auth.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository) =>
        _refreshTokenRepository = refreshTokenRepository;

    public async ValueTask<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.RefreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (storedToken is not null && storedToken.IsActive)
        {
            storedToken.Revoke();
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }

    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}