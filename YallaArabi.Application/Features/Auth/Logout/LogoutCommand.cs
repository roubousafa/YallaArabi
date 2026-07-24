using Mediator;

namespace YallaArabi.Application.Features.Auth.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Unit>;