using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Levels.GetLevelDetails;

public sealed record GetLevelDetailsQuery(string LevelCode) : IRequest<Result<LevelDetailsResponse>>;

public sealed record LevelDetailsResponse(
    string Code,
    string Name,
    string Status,
    IReadOnlyList<CategoryDetails> Categories
);

public sealed record CategoryDetails(
    string Name,
    int OrderIndex,
    IReadOnlyList<LessonDetails> Lessons
);

public sealed record LessonDetails(
    Guid LessonId,
    string Title,
    int OrderIndex,
    int PointsReward,
    bool IsCompleted
);