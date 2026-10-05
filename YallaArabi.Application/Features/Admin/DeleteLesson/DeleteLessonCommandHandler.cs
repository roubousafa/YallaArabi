using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteLesson;

public sealed class DeleteLessonCommandHandler : IRequestHandler<DeleteLessonCommand, Result<Unit>>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonContentRepository _contentRepository;

    public DeleteLessonCommandHandler(ILessonRepository lessonRepository, ILessonContentRepository contentRepository)
    {
        _lessonRepository = lessonRepository;
        _contentRepository = contentRepository;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result<Unit>.Failure("lesson_not_found", "Lesson not found");
        }

        var completions = await _lessonRepository.CountCompletionsAsync(lesson.Id, cancellationToken);

        if (completions > 0)
        {
            return Result<Unit>.Failure(
                "lesson_has_completions",
                $"Cannot delete this lesson because {completions} student(s) have already completed it. Deleting it would corrupt their progress.");
        }

        await _contentRepository.RemoveAllForLessonAsync(lesson.Id, cancellationToken);
        await _lessonRepository.RemoveAsync(lesson, cancellationToken);
        await _lessonRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}