using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetUsersList;

public sealed class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, Result<PagedUsersResponse>>
{
    private const int MaxPageSize = 100;

    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;

    public GetUsersListQueryHandler(IUserRepository userRepository, ILevelRepository levelRepository)
    {
        _userRepository = userRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<PagedUsersResponse>> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, MaxPageSize);

        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        Guid? levelId = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = allLevels.FirstOrDefault(l => l.Code == request.LevelCode);

            if (level is null)
            {
                return Result<PagedUsersResponse>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
        }

        var (items, totalCount) = await _userRepository.GetPagedAsync(
      request.SearchTerm, levelId, pageNumber, pageSize, cancellationToken);

        var results = items
            .Select(u => new AdminUserItem(
                u.Id,
                u.FullName,
                u.Email.Value,
                u.Role,
                u.CurrentLevelId.HasValue && levelCodeById.TryGetValue(u.CurrentLevelId.Value, out var code) ? code : null,
                u.SubscriptionStatus,
                u.DailyGoalMinutes,
                u.LearningMotivation,
                u.GoogleId is not null,
                u.CreatedAt))
            .ToList();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result<PagedUsersResponse>.Success(
            new PagedUsersResponse(results, totalCount, pageNumber, pageSize, totalPages));
    }
}