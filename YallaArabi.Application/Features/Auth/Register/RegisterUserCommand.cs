using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Auth.Register;

public sealed record RegisterUserCommand(string FullName, string Email, string Password) : IRequest<Result<AuthResponse>>;