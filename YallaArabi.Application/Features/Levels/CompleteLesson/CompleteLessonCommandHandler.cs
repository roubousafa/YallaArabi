using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Levels.CompleteLesson;

public sealed class CompleteLessonCommandHandler : IRequestHandler<CompleteLessonCommand, Result<CompleteLessonResponse>>
{
    private const string LessonCompletedActionKey = "lesson_completed";

    private readonly ILessonRepository _lessonRepository;
    private readonly IStudentLessonProgressRepository _progressRepository;
    private readonly IPointsService _pointsService;
    private readonly ICurrentUserService _currentUserService;

    public CompleteLessonCommandHandler(
        ILessonRepository lessonRepository,
        IStudentLessonProgressRepository progressRepository,
        IPointsService pointsService,
        ICurrentUserService currentUserService)
    {
        _lessonRepository = lessonRepository;
        _progressRepository = progressRepository;
        _pointsService = pointsService;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<CompleteLessonResponse>> Handle(CompleteLessonCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<CompleteLessonResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result<CompleteLessonResponse>.Failure("lesson_not_found", "Lesson not found");
        }

        var alreadyCompleted = await _progressRepository.ExistsAsync(currentUserId.Value, request.LessonId, cancellationToken);

        if (alreadyCompleted)
        {
            return Result<CompleteLessonResponse>.Success(new CompleteLessonResponse(lesson.Id, 0, true));
        }

        var progress = StudentLessonProgress.Complete(currentUserId.Value, lesson.Id);
        await _progressRepository.AddAsync(progress, cancellationToken);
        await _progressRepository.SaveChangesAsync(cancellationToken);

        var pointsEarned = await _pointsService.AwardPointsAsync(
            currentUserId.Value,
            LessonCompletedActionKey,
            lesson.Id,
            ct: cancellationToken);

        return Result<CompleteLessonResponse>.Success(new CompleteLessonResponse(lesson.Id, pointsEarned, false));
    }
}