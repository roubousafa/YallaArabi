using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetLessons;

public sealed class GetLessonsQueryHandler : IRequestHandler<GetLessonsQuery, Result<LessonsListResponse>>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILevelRepository _levelRepository;

    public GetLessonsQueryHandler(
        ILessonRepository lessonRepository,
        ICategoryRepository categoryRepository,
        ILevelRepository levelRepository)
    {
        _lessonRepository = lessonRepository;
        _categoryRepository = categoryRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<LessonsListResponse>> Handle(GetLessonsQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result<LessonsListResponse>.Failure("category_not_found", "Category not found");
        }

        var level = await _levelRepository.GetByIdAsync(category.LevelId, cancellationToken);
        var lessons = await _lessonRepository.GetByCategoryIdAsync(category.Id, cancellationToken);

        var items = new List<AdminLessonItem>();

        foreach (var lesson in lessons)
        {
            var completions = await _lessonRepository.CountCompletionsAsync(lesson.Id, cancellationToken);

            items.Add(new AdminLessonItem(
                lesson.Id,
                lesson.Title,
                lesson.Description,
                lesson.IconUrl,
                lesson.OrderIndex,
                lesson.PointsReward,
                completions,
                completions == 0));
        }

        return Result<LessonsListResponse>.Success(
            new LessonsListResponse(category.Id, category.Name, level?.Code, items));
    }
}