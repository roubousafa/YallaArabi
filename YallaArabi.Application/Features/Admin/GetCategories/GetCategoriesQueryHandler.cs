using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetCategories;

public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<AdminCategoryItem>>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILevelRepository _levelRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, ILevelRepository levelRepository)
    {
        _categoryRepository = categoryRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<IReadOnlyList<AdminCategoryItem>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

        if (level is null)
        {
            return Result<IReadOnlyList<AdminCategoryItem>>.Failure("level_not_found", "Level not found");
        }

        var categories = await _categoryRepository.GetByLevelIdAsync(level.Id, cancellationToken);
        var results = new List<AdminCategoryItem>();

        foreach (var category in categories)
        {
            var lessonsCount = await _categoryRepository.CountLessonsAsync(category.Id, cancellationToken);

            results.Add(new AdminCategoryItem(
                category.Id,
                category.Name,
                category.Description,
                category.IconUrl,
                category.OrderIndex,
                lessonsCount,
                lessonsCount == 0));
        }

        return Result<IReadOnlyList<AdminCategoryItem>>.Success(results);
    }
}