using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string? Description,
    string? IconUrl,
    int OrderIndex,
    string? LevelCode
) : IRequest<Result<Unit>>;