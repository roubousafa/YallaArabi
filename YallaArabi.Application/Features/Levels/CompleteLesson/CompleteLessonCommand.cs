using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Levels.CompleteLesson;

public sealed record CompleteLessonCommand(Guid LessonId) : IRequest<Result<CompleteLessonResponse>>;

public sealed record CompleteLessonResponse(Guid LessonId, int PointsEarned, bool AlreadyCompleted);