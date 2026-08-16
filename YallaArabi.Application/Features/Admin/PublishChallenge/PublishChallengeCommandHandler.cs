using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.PublishChallenge;

public sealed class PublishChallengeCommandHandler : IRequestHandler<PublishChallengeCommand, Result<Unit>>
{
    private const int MinQuestionsToPublish = 1;

    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeQuestionRepository _questionRepository;

    public PublishChallengeCommandHandler(
        IChallengeRepository challengeRepository,
        IChallengeQuestionRepository questionRepository)
    {
        _challengeRepository = challengeRepository;
        _questionRepository = questionRepository;
    }

    public async ValueTask<Result<Unit>> Handle(PublishChallengeCommand request, CancellationToken cancellationToken)
    {
        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<Unit>.Failure("challenge_not_found", "Challenge not found");
        }

        var questionsCount = await _questionRepository.CountByChallengeIdAsync(request.ChallengeId, cancellationToken);

        if (questionsCount < MinQuestionsToPublish)
        {
            return Result<Unit>.Failure(
                "no_questions",
                "Cannot publish a challenge without questions. Please add at least one question first.");
        }

        challenge.Publish();
        await _challengeRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}