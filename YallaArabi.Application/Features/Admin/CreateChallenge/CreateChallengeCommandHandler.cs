using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.CreateChallenge;

public sealed class CreateChallengeCommandHandler : IRequestHandler<CreateChallengeCommand, Result<CreateChallengeResponse>>
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateChallengeCommandHandler(
        IChallengeRepository challengeRepository,
        ILevelRepository levelRepository,
        ICurrentUserService currentUserService)
    {
        _challengeRepository = challengeRepository;
        _levelRepository = levelRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<CreateChallengeResponse>> Handle(CreateChallengeCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<CreateChallengeResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        Guid? levelId = null;
        string? levelCode = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

            if (level is null)
            {
                return Result<CreateChallengeResponse>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
            levelCode = level.Code;
        }

        var challenge = Challenge.Create(
            request.Title,
            request.Description,
            levelId,
            request.PointsReward,
            request.TimeLimitSeconds,
            currentUserId.Value,
            request.ExpiresAt);

        await _challengeRepository.AddAsync(challenge, cancellationToken);
        await _challengeRepository.SaveChangesAsync(cancellationToken);

        return Result<CreateChallengeResponse>.Success(
            new CreateChallengeResponse(challenge.Id, challenge.Title, levelCode, challenge.Status));
    }
}