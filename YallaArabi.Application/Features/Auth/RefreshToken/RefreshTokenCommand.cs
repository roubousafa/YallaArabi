using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;