using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateLesson;

public sealed class UpdateLessonCommandHandler : IRequestHandler<UpdateLessonCommand, Result<Unit>>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateLessonCommandHandler(
        ILessonRepository lessonRepository,
        ICategoryRepository categoryRepository)
    {
        _lessonRepository = lessonRepository;
        _categoryRepository = categoryRepository;
    }

    public async ValueTask<Result<Unit>> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result<Unit>.Failure("lesson_not_found", "Lesson not found");
        }

        if (request.CategoryId.HasValue && request.CategoryId.Value != lesson.CategoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);

            if (category is null)
            {
                return Result<Unit>.Failure("category_not_found", "Category not found");
            }

            lesson.MoveToCategory(category.Id);
        }

        lesson.Update(
            request.Title,
            request.Description,
            request.IconUrl,
            request.OrderIndex,
            request.PointsReward);

        await _lessonRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}