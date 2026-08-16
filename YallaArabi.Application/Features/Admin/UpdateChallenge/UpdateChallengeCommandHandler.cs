using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateChallenge;

public sealed class UpdateChallengeCommandHandler : IRequestHandler<UpdateChallengeCommand, Result<Unit>>
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly ILevelRepository _levelRepository;

    public UpdateChallengeCommandHandler(IChallengeRepository challengeRepository, ILevelRepository levelRepository)
    {
        _challengeRepository = challengeRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<Unit>> Handle(UpdateChallengeCommand request, CancellationToken cancellationToken)
    {
        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<Unit>.Failure("challenge_not_found", "Challenge not found");
        }

        Guid? levelId = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

            if (level is null)
            {
                return Result<Unit>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
        }

        challenge.Update(
            request.Title,
            request.Description,
            levelId,
            request.PointsReward,
            request.TimeLimitSeconds,
            request.ExpiresAt);

        await _challengeRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}