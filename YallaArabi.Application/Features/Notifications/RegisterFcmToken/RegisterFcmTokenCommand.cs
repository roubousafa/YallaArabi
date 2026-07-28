using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Notifications.RegisterFcmToken;

public sealed record RegisterFcmTokenCommand(string FcmToken) : IRequest<Result<Unit>>;