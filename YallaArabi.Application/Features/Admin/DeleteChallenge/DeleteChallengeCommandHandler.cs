using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteChallenge;

public sealed class DeleteChallengeCommandHandler : IRequestHandler<DeleteChallengeCommand, Result<Unit>>
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeQuestionRepository _questionRepository;

    public DeleteChallengeCommandHandler(
        IChallengeRepository challengeRepository,
        IChallengeQuestionRepository questionRepository)
    {
        _challengeRepository = challengeRepository;
        _questionRepository = questionRepository;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteChallengeCommand request, CancellationToken cancellationToken)
    {
        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<Unit>.Failure("challenge_not_found", "Challenge not found");
        }

        var questions = await _questionRepository.GetByChallengeIdAsync(challenge.Id, cancellationToken);

        foreach (var question in questions)
        {
            await _questionRepository.RemoveAsync(question, cancellationToken);
        }

        await _challengeRepository.RemoveAsync(challenge, cancellationToken);
        await _challengeRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}