using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetCategories;

public sealed record GetCategoriesQuery(string LevelCode) : IRequest<Result<IReadOnlyList<AdminCategoryItem>>>;

public sealed record AdminCategoryItem(
    Guid CategoryId,
    string Name,
    string? Description,
    string? IconUrl,
    int OrderIndex,
    int LessonsCount,
    bool CanBeDeleted
);