using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Levels.GetLevelsMap;

public sealed record GetLevelsMapQuery : IRequest<Result<IReadOnlyList<LevelMapItem>>>;

public sealed record LevelMapItem(
    string Code,
    string Name,
    int OrderIndex,
    string Status,
    int TotalLessons,
    int CompletedLessons,
    int CompletionPercentage,
    bool IsPremium,
    bool IsCurrent
);