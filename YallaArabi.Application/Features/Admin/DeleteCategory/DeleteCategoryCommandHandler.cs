using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteCategory;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<Unit>>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result<Unit>.Failure("category_not_found", "Category not found");
        }

        var lessonsCount = await _categoryRepository.CountLessonsAsync(category.Id, cancellationToken);

        if (lessonsCount > 0)
        {
            return Result<Unit>.Failure(
                "category_has_lessons",
                $"Cannot delete this category because it still has {lessonsCount} lesson(s). Please move or delete them first.");
        }

        await _categoryRepository.RemoveAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}