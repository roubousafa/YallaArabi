using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Auth.Login;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;