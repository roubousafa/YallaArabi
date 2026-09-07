using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteLesson;

public sealed record DeleteLessonCommand(Guid LessonId) : IRequest<Result<Unit>>;