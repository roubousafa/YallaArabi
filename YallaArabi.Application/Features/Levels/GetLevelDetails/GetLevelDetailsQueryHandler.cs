using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Levels.GetLevelDetails;

public sealed class GetLevelDetailsQueryHandler : IRequestHandler<GetLevelDetailsQuery, Result<LevelDetailsResponse>>
{
    private const string FreeLevelCode = "A0";

    private readonly ILevelRepository _levelRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IStudentLessonProgressRepository _progressRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetLevelDetailsQueryHandler(
        ILevelRepository levelRepository,
        ICategoryRepository categoryRepository,
        ILessonRepository lessonRepository,
        IStudentLessonProgressRepository progressRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _levelRepository = levelRepository;
        _categoryRepository = categoryRepository;
        _lessonRepository = lessonRepository;
        _progressRepository = progressRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<LevelDetailsResponse>> Handle(GetLevelDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<LevelDetailsResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

        if (level is null)
        {
            return Result<LevelDetailsResponse>.Failure("level_not_found", "Level not found");
        }

        if (level.Code != FreeLevelCode)
        {
            var activeSubscription = await _subscriptionRepository.GetActiveByUserIdAsync(currentUserId.Value, cancellationToken);
            var hasPremium = activeSubscription is not null && activeSubscription.IsCurrentlyActive();

            if (!hasPremium)
            {
                return Result<LevelDetailsResponse>.Failure("premium_required", "This level requires a premium subscription");
            }
        }

        var categories = await _categoryRepository.GetByLevelIdAsync(level.Id, cancellationToken);
        var categoryIds = categories.Select(c => c.Id).ToList();
        var lessons = await _lessonRepository.GetByCategoryIdsAsync(categoryIds, cancellationToken);

        var completedLessonIds = (await _progressRepository.GetByUserIdAsync(currentUserId.Value, cancellationToken))
            .Select(p => p.LessonId)
            .ToHashSet();

        var categoryDetailsList = categories
            .OrderBy(c => c.OrderIndex)
            .Select(category => new CategoryDetails(
                category.Name,
                category.OrderIndex,
                lessons
                    .Where(l => l.CategoryId == category.Id)
                    .OrderBy(l => l.OrderIndex)
                    .Select(l => new LessonDetails(
                        l.Id,
                        l.Title,
                        l.OrderIndex,
                        l.PointsReward,
                        completedLessonIds.Contains(l.Id)))
                    .ToList()))
            .ToList();

        var totalLessons = lessons.Count;
        var completedCount = lessons.Count(l => completedLessonIds.Contains(l.Id));
        var status = totalLessons > 0 && completedCount == totalLessons ? "completed" : "unlocked";

        var response = new LevelDetailsResponse(level.Code, level.Name, status, categoryDetailsList);
        return Result<LevelDetailsResponse>.Success(response);
    }
}