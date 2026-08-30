using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateCategory;

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<Unit>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILevelRepository _levelRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, ILevelRepository levelRepository)
    {
        _categoryRepository = categoryRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<Unit>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result<Unit>.Failure("category_not_found", "Category not found");
        }

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

            if (level is null)
            {
                return Result<Unit>.Failure("level_not_found", "Level not found");
            }

            category.MoveToLevel(level.Id);
        }

        category.Update(request.Name, request.Description, request.IconUrl, request.OrderIndex);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}