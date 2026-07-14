namespace YallaArabi.Application.Common;

public sealed record AuthResponse(Guid UserId, string FullName, string Email, string AccessToken, string RefreshToken);