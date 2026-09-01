using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateTeamMember;

public sealed record UpdateTeamMemberCommand(
    Guid UserId,
    string FullName,
    Guid RoleId
) : IRequest<Result<Unit>>;