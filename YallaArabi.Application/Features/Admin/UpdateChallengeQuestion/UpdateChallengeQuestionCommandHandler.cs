using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateChallengeQuestion;

public sealed class UpdateChallengeQuestionCommandHandler : IRequestHandler<UpdateChallengeQuestionCommand, Result<Unit>>
{
    private readonly IChallengeQuestionRepository _questionRepository;

    public UpdateChallengeQuestionCommandHandler(IChallengeQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async ValueTask<Result<Unit>> Handle(UpdateChallengeQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);

        if (question is null)
        {
            return Result<Unit>.Failure("question_not_found", "Question not found");
        }

        question.Update(
            request.QuestionText,
            request.OptionA,
            request.OptionB,
            request.OptionC,
            request.OptionD,
            request.CorrectOption,
            request.OrderIndex,
            request.AudioUrl,
            request.ImageUrl);

        await _questionRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}