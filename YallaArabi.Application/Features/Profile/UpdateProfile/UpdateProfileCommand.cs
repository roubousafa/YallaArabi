using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Profile.UpdateProfile;

public sealed record UpdateProfileCommand(string FullName) : IRequest<Result<UpdateProfileResponse>>;

public sealed record UpdateProfileResponse(Guid UserId, string FullName, string Email);