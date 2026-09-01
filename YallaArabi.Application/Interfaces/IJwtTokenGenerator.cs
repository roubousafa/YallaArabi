using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user, IReadOnlyList<string>? permissions = null);
    string GenerateRefreshToken();
}