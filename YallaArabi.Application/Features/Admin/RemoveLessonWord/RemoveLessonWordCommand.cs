using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.RemoveLessonWord;

public sealed record RemoveLessonWordCommand(Guid LessonId, Guid WordId) : IRequest<Result<Unit>>;

public sealed class RemoveLessonWordCommandHandler : IRequestHandler<RemoveLessonWordCommand, Result<Unit>>
{
    private readonly ILessonContentRepository _contentRepository;

    public RemoveLessonWordCommandHandler(ILessonContentRepository contentRepository) =>
        _contentRepository = contentRepository;

    public async ValueTask<Result<Unit>> Handle(RemoveLessonWordCommand request, CancellationToken cancellationToken)
    {
        var link = await _contentRepository.GetWordAsync(request.LessonId, request.WordId, cancellationToken);

        if (link is null)
        {
            return Result<Unit>.Failure("word_not_in_lesson", "This word is not part of the lesson");
        }

        _contentRepository.RemoveWord(link);
        await _contentRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
