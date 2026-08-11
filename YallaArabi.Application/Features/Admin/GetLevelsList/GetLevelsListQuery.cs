using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetLevelsList;

public sealed record GetLevelsListQuery : IRequest<Result<IReadOnlyList<AdminLevelItem>>>;

public sealed record AdminLevelItem(
    Guid LevelId,
    string Code,
    string Name,
    int OrderIndex,
    int CategoriesCount,
    int LessonsCount,
    int WordsCount,
    int StudentsCount,
    bool CanBeDeleted
);