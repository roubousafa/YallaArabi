using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Levels.GetLessonContent;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetLessonContent;

public sealed record GetLessonContentForAdminQuery(Guid LessonId) : IRequest<Result<LessonContentResponse>>;

public sealed class GetLessonContentForAdminQueryHandler : IRequestHandler<GetLessonContentForAdminQuery, Result<LessonContentResponse>>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonContentService _lessonContentService;

    public GetLessonContentForAdminQueryHandler(ILessonRepository lessonRepository, ILessonContentService lessonContentService)
    {
        _lessonRepository = lessonRepository;
        _lessonContentService = lessonContentService;
    }

    public async ValueTask<Result<LessonContentResponse>> Handle(GetLessonContentForAdminQuery request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result<LessonContentResponse>.Failure("lesson_not_found", "Lesson not found");
        }

        var response = await _lessonContentService.BuildAsync(lesson, isCompleted: false, studentId: null, cancellationToken);
        return Result<LessonContentResponse>.Success(response);
    }
}
