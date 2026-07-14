using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Auth.GoogleLogin;

public sealed record LoginWithGoogleCommand(string IdToken) : IRequest<Result<AuthResponse>>;