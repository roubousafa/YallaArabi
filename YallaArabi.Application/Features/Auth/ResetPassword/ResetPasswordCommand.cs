using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Auth.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result<Unit>>;