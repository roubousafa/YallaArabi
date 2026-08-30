using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.AddChallengeQuestion;

public sealed class AddChallengeQuestionCommandHandler : IRequestHandler<AddChallengeQuestionCommand, Result<AddChallengeQuestionResponse>>
{
    private const int MaxQuestionsPerChallenge = 50;

    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeQuestionRepository _questionRepository;

    public AddChallengeQuestionCommandHandler(
        IChallengeRepository challengeRepository,
        IChallengeQuestionRepository questionRepository)
    {
        _challengeRepository = challengeRepository;
        _questionRepository = questionRepository;
    }

    public async ValueTask<Result<AddChallengeQuestionResponse>> Handle(AddChallengeQuestionCommand request, CancellationToken cancellationToken)
    {
        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<AddChallengeQuestionResponse>.Failure("challenge_not_found", "Challenge not found");
        }

        var currentCount = await _questionRepository.CountByChallengeIdAsync(request.ChallengeId, cancellationToken);

        if (currentCount >= MaxQuestionsPerChallenge)
        {
            return Result<AddChallengeQuestionResponse>.Failure(
                "max_questions_reached",
                $"A challenge cannot have more than {MaxQuestionsPerChallenge} questions");
        }

        var question = ChallengeQuestion.Create(
            request.ChallengeId,
            request.QuestionText,
            request.OptionA,
            request.OptionB,
            request.OptionC,
            request.OptionD,
            request.CorrectOption,
            currentCount,
            request.AudioUrl,
            request.ImageUrl);

        await _questionRepository.AddAsync(question, cancellationToken);
        await _questionRepository.SaveChangesAsync(cancellationToken);

        return Result<AddChallengeQuestionResponse>.Success(
            new AddChallengeQuestionResponse(question.Id, challenge.Id, question.OrderIndex, currentCount + 1));
    }
}