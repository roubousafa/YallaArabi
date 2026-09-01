using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.CreateTeamMember;

public sealed record CreateTeamMemberCommand(
    string FullName,
    string Email,
    string Password,
    Guid RoleId
) : IRequest<Result<CreateTeamMemberResponse>>;

public sealed record CreateTeamMemberResponse(
    Guid UserId,
    string FullName,
    string Email,
    Guid RoleId,
    string RoleNameEn
);