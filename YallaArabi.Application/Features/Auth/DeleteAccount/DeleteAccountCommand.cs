using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Auth.DeleteAccount;

public sealed record DeleteAccountCommand(string? Password) : IRequest<Result<Unit>>;