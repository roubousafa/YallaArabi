using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteTeamMember;

public sealed record DeleteTeamMemberCommand(Guid UserId) : IRequest<Result<Unit>>;