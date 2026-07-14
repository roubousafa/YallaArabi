using Mediator;

namespace YallaArabi.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Unit>;