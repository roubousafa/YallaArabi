using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class LessonWord : Entity
{
    public Guid LessonId { get; private set; }
    public Guid WordId { get; private set; }
    public int OrderIndex { get; private set; }

    private LessonWord() { }

    public static LessonWord Create(Guid lessonId, Guid wordId, int orderIndex)
    {
        return new LessonWord
        {
            Id = Guid.NewGuid(),
            LessonId = lessonId,
            WordId = wordId,
            OrderIndex = orderIndex
        };
    }
}
