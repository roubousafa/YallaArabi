using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Levels.GetLevelsMap;

public sealed class GetLevelsMapQueryHandler : IRequestHandler<GetLevelsMapQuery, Result<IReadOnlyList<LevelMapItem>>>
{
    private const string FreeLevelCode = "A0";

    private readonly ILevelRepository _levelRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IStudentLessonProgressRepository _progressRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetLevelsMapQueryHandler(
        ILevelRepository levelRepository,
        ICategoryRepository categoryRepository,
        ILessonRepository lessonRepository,
        IStudentLessonProgressRepository progressRepository,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _levelRepository = levelRepository;
        _categoryRepository = categoryRepository;
        _lessonRepository = lessonRepository;
        _progressRepository = progressRepository;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<IReadOnlyList<LevelMapItem>>> Handle(GetLevelsMapQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<IReadOnlyList<LevelMapItem>>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<IReadOnlyList<LevelMapItem>>.Failure("user_not_found", "User not found");
        }

        var activeSubscription = await _subscriptionRepository.GetActiveByUserIdAsync(currentUserId.Value, cancellationToken);
        var hasPremium = activeSubscription is not null && activeSubscription.IsCurrentlyActive();

        var levels = await _levelRepository.GetAllAsync(cancellationToken);

        var completedLessonIds = (await _progressRepository.GetByUserIdAsync(currentUserId.Value, cancellationToken))
            .Select(p => p.LessonId)
            .ToHashSet();

        var result = new List<LevelMapItem>();

        foreach (var level in levels)
        {
            var categories = await _categoryRepository.GetByLevelIdAsync(level.Id, cancellationToken);
            var categoryIds = categories.Select(c => c.Id).ToList();
            var lessons = await _lessonRepository.GetByCategoryIdsAsync(categoryIds, cancellationToken);

            var totalLessons = lessons.Count;
            var completedCount = lessons.Count(l => completedLessonIds.Contains(l.Id));
            var isLevelFullyCompleted = totalLessons > 0 && completedCount == totalLessons;

            var isPremiumLevel = level.Code != FreeLevelCode;
            var isAccessible = !isPremiumLevel || hasPremium;

            string status;
            if (!isAccessible)
                status = "locked";
            else if (isLevelFullyCompleted)
                status = "completed";
            else
                status = "unlocked";

            var percentage = totalLessons == 0 ? 0 : (int)Math.Round(completedCount * 100.0 / totalLessons);
            var isCurrent = user.CurrentLevelId == level.Id;

            result.Add(new LevelMapItem(
                level.Code,
                level.Name,
                level.OrderIndex,
                status,
                totalLessons,
                completedCount,
                percentage,
                isPremiumLevel,
                isCurrent));
        }

        return Result<IReadOnlyList<LevelMapItem>>.Success(result);
    }
}