using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Levels.GetLessonContent;

public sealed class GetLessonContentQueryHandler : IRequestHandler<GetLessonContentQuery, Result<LessonContentResponse>>
{
    private const string FreeLevelCode = "A0";

    private readonly ILessonRepository _lessonRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IStudentLessonProgressRepository _progressRepository;
    private readonly ILessonContentService _lessonContentService;
    private readonly ICurrentUserService _currentUserService;

    public GetLessonContentQueryHandler(
        ILessonRepository lessonRepository,
        ICategoryRepository categoryRepository,
        ILevelRepository levelRepository,
        ISubscriptionRepository subscriptionRepository,
        IStudentLessonProgressRepository progressRepository,
        ILessonContentService lessonContentService,
        ICurrentUserService currentUserService)
    {
        _lessonRepository = lessonRepository;
        _categoryRepository = categoryRepository;
        _levelRepository = levelRepository;
        _subscriptionRepository = subscriptionRepository;
        _progressRepository = progressRepository;
        _lessonContentService = lessonContentService;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<LessonContentResponse>> Handle(GetLessonContentQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<LessonContentResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result<LessonContentResponse>.Failure("lesson_not_found", "Lesson not found");
        }

        var category = await _categoryRepository.GetByIdAsync(lesson.CategoryId, cancellationToken);
        var level = category is null ? null : await _levelRepository.GetByIdAsync(category.LevelId, cancellationToken);

        if (level is null)
        {
            return Result<LessonContentResponse>.Failure("lesson_not_found", "Lesson not found");
        }

        // Same rule as the level details screen: only A0 is free.
        if (level.Code != FreeLevelCode)
        {
            var activeSubscription = await _subscriptionRepository.GetActiveByUserIdAsync(currentUserId.Value, cancellationToken);
            var hasPremium = activeSubscription is not null && activeSubscription.IsCurrentlyActive();

            if (!hasPremium)
            {
                return Result<LessonContentResponse>.Failure("premium_required", "This lesson requires a premium subscription");
            }
        }

        var isCompleted = await _progressRepository.ExistsAsync(currentUserId.Value, lesson.Id, cancellationToken);
        var response = await _lessonContentService.BuildAsync(lesson, isCompleted, currentUserId.Value, cancellationToken);

        return Result<LessonContentResponse>.Success(response);
    }
}
