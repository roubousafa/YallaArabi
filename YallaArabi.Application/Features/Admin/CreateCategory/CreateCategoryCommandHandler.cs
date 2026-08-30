using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.CreateCategory;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private const int MaxCategoriesPerLevel = 50;

    private readonly ICategoryRepository _categoryRepository;
    private readonly ILevelRepository _levelRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, ILevelRepository levelRepository)
    {
        _categoryRepository = categoryRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

        if (level is null)
        {
            return Result<CreateCategoryResponse>.Failure("level_not_found", "Level not found");
        }

        var currentCount = await _categoryRepository.CountByLevelIdAsync(level.Id, cancellationToken);

        if (currentCount >= MaxCategoriesPerLevel)
        {
            return Result<CreateCategoryResponse>.Failure(
                "max_categories_reached",
                $"A level cannot have more than {MaxCategoriesPerLevel} categories");
        }

        var category = Category.Create(
            Guid.NewGuid(),
            level.Id,
            request.Name,
            currentCount,
            request.Description,
            request.IconUrl);

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return Result<CreateCategoryResponse>.Success(
            new CreateCategoryResponse(category.Id, category.Name, level.Code, category.OrderIndex));
    }
}