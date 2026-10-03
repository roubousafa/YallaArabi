using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Auth.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword
) : IRequest<Result<Unit>>;