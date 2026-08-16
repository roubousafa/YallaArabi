using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteChallengeQuestion;

public sealed class DeleteChallengeQuestionCommandHandler : IRequestHandler<DeleteChallengeQuestionCommand, Result<Unit>>
{
    private readonly IChallengeQuestionRepository _questionRepository;

    public DeleteChallengeQuestionCommandHandler(IChallengeQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteChallengeQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);

        if (question is null)
        {
            return Result<Unit>.Failure("question_not_found", "Question not found");
        }

        await _questionRepository.RemoveAsync(question, cancellationToken);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}