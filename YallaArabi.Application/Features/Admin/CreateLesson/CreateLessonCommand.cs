using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.CreateLesson;

public sealed record CreateLessonCommand(
    Guid CategoryId,
    string Title,
    string? Description,
    string? IconUrl,
    int PointsReward = 10
) : IRequest<Result<CreateLessonResponse>>;

public sealed record CreateLessonResponse(
    Guid LessonId,
    string Title,
    Guid CategoryId,
    int OrderIndex
);