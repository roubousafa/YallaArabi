using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetTeamList;

public sealed class GetTeamListQueryHandler : IRequestHandler<GetTeamListQuery, Result<PagedTeamResponse>>
{
    private const int MaxPageSize = 100;

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public GetTeamListQueryHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async ValueTask<Result<PagedTeamResponse>> Handle(GetTeamListQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, MaxPageSize);

        var allRoles = await _roleRepository.GetAllAsync(cancellationToken);
        var rolesById = allRoles.ToDictionary(r => r.Id);

        var (items, totalCount) = await _userRepository.GetPagedTeamAsync(
            request.SearchTerm, request.RoleId, pageNumber, pageSize, cancellationToken);

        var results = items
            .Select(u =>
            {
                var role = u.RoleId.HasValue && rolesById.TryGetValue(u.RoleId.Value, out var r) ? r : null;

                return new TeamMemberItem(
                    u.Id,
                    u.FullName,
                    u.Email.Value,
                    u.RoleId,
                    role?.NameEn,
                    role?.NameAr,
                    role?.IsSystemRole ?? false,
                    u.GoogleId is not null,
                    u.MustChangePassword,
                    u.CreatedAt);
            })
            .ToList();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result<PagedTeamResponse>.Success(
            new PagedTeamResponse(results, totalCount, pageNumber, pageSize, totalPages));
    }
}