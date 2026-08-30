using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.CreateCategory;

public sealed record CreateCategoryCommand(
    string LevelCode,
    string Name,
    string? Description,
    string? IconUrl
) : IRequest<Result<CreateCategoryResponse>>;

public sealed record CreateCategoryResponse(
    Guid CategoryId,
    string Name,
    string LevelCode,
    int OrderIndex
);