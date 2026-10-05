using YallaArabi.Application.Features.Levels.GetLessonContent;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface ILessonContentService
{
    // studentId is null for the admin view, which has no per-student progress.
    Task<LessonContentResponse> BuildAsync(Lesson lesson, bool isCompleted, Guid? studentId, CancellationToken ct = default);
}
