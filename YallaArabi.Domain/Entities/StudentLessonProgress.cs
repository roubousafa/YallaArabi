using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class StudentLessonProgress : AggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid LessonId { get; private set; }
    public DateTime CompletedAt { get; private set; }

    private StudentLessonProgress() { }

    public static StudentLessonProgress Complete(Guid userId, Guid lessonId)
    {
        return new StudentLessonProgress
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            LessonId = lessonId,
            CompletedAt = DateTime.UtcNow
        };
    }
}