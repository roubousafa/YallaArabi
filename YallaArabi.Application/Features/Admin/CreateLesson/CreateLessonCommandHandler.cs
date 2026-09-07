using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.CreateLesson;

public sealed class CreateLessonCommandHandler : IRequestHandler<CreateLessonCommand, Result<CreateLessonResponse>>
{
    private const int MaxLessonsPerCategory = 100;

    private readonly ILessonRepository _lessonRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateLessonCommandHandler(
        ILessonRepository lessonRepository,
        ICategoryRepository categoryRepository)
    {
        _lessonRepository = lessonRepository;
        _categoryRepository = categoryRepository;
    }

    public async ValueTask<Result<CreateLessonResponse>> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result<CreateLessonResponse>.Failure("category_not_found", "Category not found");
        }

        var currentCount = await _lessonRepository.CountByCategoryIdAsync(category.Id, cancellationToken);

        if (currentCount >= MaxLessonsPerCategory)
        {
            return Result<CreateLessonResponse>.Failure(
                "max_lessons_reached",
                $"A category cannot have more than {MaxLessonsPerCategory} lessons");
        }

        var lesson = Lesson.Create(
            Guid.NewGuid(),
            category.Id,
            request.Title,
            currentCount,
            request.PointsReward,
            request.Description,
            request.IconUrl);

        await _lessonRepository.AddAsync(lesson, cancellationToken);
        await _lessonRepository.SaveChangesAsync(cancellationToken);

        return Result<CreateLessonResponse>.Success(
            new CreateLessonResponse(lesson.Id, lesson.Title, category.Id, lesson.OrderIndex));
    }
}