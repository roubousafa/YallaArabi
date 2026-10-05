using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateLessonQuestion;

public sealed class UpdateLessonQuestionCommandHandler : IRequestHandler<UpdateLessonQuestionCommand, Result<Unit>>
{
    private readonly ILessonContentRepository _contentRepository;
    private readonly IWordRepository _wordRepository;

    public UpdateLessonQuestionCommandHandler(ILessonContentRepository contentRepository, IWordRepository wordRepository)
    {
        _contentRepository = contentRepository;
        _wordRepository = wordRepository;
    }

    public async ValueTask<Result<Unit>> Handle(UpdateLessonQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _contentRepository.GetQuestionByIdAsync(request.QuestionId, cancellationToken);

        if (question is null)
        {
            return Result<Unit>.Failure("question_not_found", "Question not found");
        }

        if (request.WordId.HasValue && await _wordRepository.GetByIdAsync(request.WordId.Value, cancellationToken) is null)
        {
            return Result<Unit>.Failure("word_not_found", "Word not found");
        }

        question.Update(
            request.Type,
            request.QuestionText,
            request.OptionA,
            request.OptionB,
            request.OptionC,
            request.OptionD,
            request.CorrectOption,
            request.WordId,
            request.AudioUrl,
            request.ImageUrl,
            request.OrderIndex);

        await _contentRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
