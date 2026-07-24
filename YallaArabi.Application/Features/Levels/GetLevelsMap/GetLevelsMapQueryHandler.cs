using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Levels.GetLevelsMap;

public sealed class GetLevelsMapQueryHandler : IRequestHandler<GetLevelsMapQuery, Result<IReadOnlyList<LevelMapItem>>>
{
    private readonly ILevelRepository _levelRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IStudentLessonProgressRepository _progressRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetLevelsMapQueryHandler(
        ILevelRepository levelRepository,
        ICategoryRepository categoryRepository,
        ILessonRepository lessonRepository,
        IStudentLessonProgressRepository progressRepository,
        ICurrentUserService currentUserService)
    {
        _levelRepository = levelRepository;
        _categoryRepository = categoryRepository;
        _lessonRepository = lessonRepository;
        _progressRepository = progressRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<IReadOnlyList<LevelMapItem>>> Handle(GetLevelsMapQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<IReadOnlyList<LevelMapItem>>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var levels = await _levelRepository.GetAllAsync(cancellationToken);
        var completedLessonIds = (await _progressRepository.GetByUserIdAsync(currentUserId.Value, cancellationToken))
            .Select(p => p.LessonId)
            .ToHashSet();

        var result = new List<LevelMapItem>();
        var previousLevelCompleted = true;

        foreach (var level in levels)
        {
            var categories = await _categoryRepository.GetByLevelIdAsync(level.Id, cancellationToken);
            var categoryIds = categories.Select(c => c.Id).ToList();
            var lessons = await _lessonRepository.GetByCategoryIdsAsync(categoryIds, cancellationToken);

            var totalLessons = lessons.Count;
            var completedCount = lessons.Count(l => completedLessonIds.Contains(l.Id));
            var isLevelFullyCompleted = totalLessons > 0 && completedCount == totalLessons;

            string status;
            if (isLevelFullyCompleted)
                status = "completed";
            else if (previousLevelCompleted)
                status = "unlocked";
            else
                status = "locked";

            var percentage = totalLessons == 0 ? 0 : (int)Math.Round(completedCount * 100.0 / totalLessons);

            result.Add(new LevelMapItem(
                level.Code,
                level.Name,
                level.OrderIndex,
                status,
                totalLessons,
                completedCount,
                percentage));

            previousLevelCompleted = isLevelFullyCompleted;
        }

        return Result<IReadOnlyList<LevelMapItem>>.Success(result);
    }
}