using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteWord;

public sealed class DeleteWordCommandHandler : IRequestHandler<DeleteWordCommand, Result<Unit>>
{
    private readonly IWordRepository _wordRepository;
    private readonly ILessonContentRepository _lessonContentRepository;

    public DeleteWordCommandHandler(IWordRepository wordRepository, ILessonContentRepository lessonContentRepository)
    {
        _wordRepository = wordRepository;
        _lessonContentRepository = lessonContentRepository;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteWordCommand request, CancellationToken cancellationToken)
    {
        var word = await _wordRepository.GetByIdAsync(request.WordId, cancellationToken);

        if (word is null)
        {
            return Result<Unit>.Failure("word_not_found", "Word not found");
        }

        if (await _lessonContentRepository.IsWordUsedInQuestionsAsync(word.Id, cancellationToken))
        {
            return Result<Unit>.Failure(
                "word_used_in_lesson_questions",
                "This word is used by lesson questions. Remove or edit those questions first.");
        }

        await _lessonContentRepository.RemoveWordFromAllLessonsAsync(word.Id, cancellationToken);
        await _wordRepository.RemoveAsync(word, cancellationToken);
        await _wordRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}