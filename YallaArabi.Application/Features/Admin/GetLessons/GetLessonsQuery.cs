using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetLessons;

public sealed record GetLessonsQuery(Guid CategoryId) : IRequest<Result<LessonsListResponse>>;

public sealed record LessonsListResponse(
    Guid CategoryId,
    string CategoryName,
    string? LevelCode,
    IReadOnlyList<AdminLessonItem> Lessons
);

public sealed record AdminLessonItem(
    Guid LessonId,
    string Title,
    string? Description,
    string? IconUrl,
    int OrderIndex,
    int PointsReward,
    int CompletionsCount,
    bool CanBeDeleted
);