using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteLessonQuestion;

public sealed record DeleteLessonQuestionCommand(Guid QuestionId) : IRequest<Result<Unit>>;

public sealed class DeleteLessonQuestionCommandHandler : IRequestHandler<DeleteLessonQuestionCommand, Result<Unit>>
{
    private readonly ILessonContentRepository _contentRepository;

    public DeleteLessonQuestionCommandHandler(ILessonContentRepository contentRepository) =>
        _contentRepository = contentRepository;

    public async ValueTask<Result<Unit>> Handle(DeleteLessonQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _contentRepository.GetQuestionByIdAsync(request.QuestionId, cancellationToken);

        if (question is null)
        {
            return Result<Unit>.Failure("question_not_found", "Question not found");
        }

        _contentRepository.RemoveQuestion(question);
        await _contentRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
