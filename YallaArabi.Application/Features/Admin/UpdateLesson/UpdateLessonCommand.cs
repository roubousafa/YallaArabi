using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateLesson;

public sealed record UpdateLessonCommand(
    Guid LessonId,
    string Title,
    string? Description,
    string? IconUrl,
    int OrderIndex,
    int PointsReward,
    Guid? CategoryId
) : IRequest<Result<Unit>>;