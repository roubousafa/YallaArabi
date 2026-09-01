using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetTeamList;

public sealed record GetTeamListQuery(
    string? SearchTerm,
    Guid? RoleId,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedTeamResponse>>;

public sealed record PagedTeamResponse(
    IReadOnlyList<TeamMemberItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public sealed record TeamMemberItem(
    Guid UserId,
    string FullName,
    string Email,
    Guid? RoleId,
    string? RoleNameEn,
    string? RoleNameAr,
    bool IsSystemRole,
    bool HasGoogleAccount,
    bool MustChangePassword,
    DateTime CreatedAt
);