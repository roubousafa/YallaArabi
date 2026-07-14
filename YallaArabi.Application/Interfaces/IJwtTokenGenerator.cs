using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}