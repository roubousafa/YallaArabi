using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Challenges.GetAvailableChallenges;

public sealed class GetAvailableChallengesQueryHandler : IRequestHandler<GetAvailableChallengesQuery, Result<IReadOnlyList<AvailableChallengeItem>>>
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeQuestionRepository _questionRepository;
    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IChallengeAssignmentRepository _assignmentRepository;

    public GetAvailableChallengesQueryHandler(
        IChallengeRepository challengeRepository,
        IChallengeQuestionRepository questionRepository,
        IChallengeParticipationRepository participationRepository,
        IUserRepository userRepository,
         ILevelRepository levelRepository,
         IChallengeAssignmentRepository assignmentRepository,
        ICurrentUserService currentUserService)
    {
        _challengeRepository = challengeRepository;
        _questionRepository = questionRepository;
        _participationRepository = participationRepository;
        _userRepository = userRepository;
        _levelRepository = levelRepository;
        _currentUserService = currentUserService;
        _assignmentRepository = assignmentRepository;
    }

    public async ValueTask<Result<IReadOnlyList<AvailableChallengeItem>>> Handle(GetAvailableChallengesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<IReadOnlyList<AvailableChallengeItem>>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<IReadOnlyList<AvailableChallengeItem>>.Failure("user_not_found", "User not found");
        }

        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var allowedLevelIds = new List<Guid>();

        if (user.CurrentLevelId.HasValue)
        {
            var currentLevel = allLevels.FirstOrDefault(l => l.Id == user.CurrentLevelId.Value);

            if (currentLevel is not null)
            {
                allowedLevelIds = allLevels
                    .Where(l => l.OrderIndex <= currentLevel.OrderIndex)
                    .Select(l => l.Id)
                    .ToList();
            }
        }

        var challenges = await _challengeRepository.GetAvailableForLevelsAsync(allowedLevelIds, cancellationToken);
        var myParticipations = await _participationRepository.GetByUserIdAsync(currentUserId.Value, cancellationToken);

        var results = new List<AvailableChallengeItem>();

        foreach (var challenge in challenges)
        {
            var questionsCount = await _questionRepository.CountByChallengeIdAsync(challenge.Id, cancellationToken);

            if (questionsCount == 0)
            {
                continue;
            }

            var completedParticipation = myParticipations
                .FirstOrDefault(p => p.ChallengeId == challenge.Id && p.IsCompleted());

            results.Add(new AvailableChallengeItem(
                challenge.Id,
                challenge.Title,
                challenge.Description,
                challenge.PointsReward,
                challenge.TimeLimitSeconds,
                questionsCount,
                completedParticipation is not null,
                completedParticipation?.ScorePercentage(),
                challenge.ExpiresAt));
        }

        return Result<IReadOnlyList<AvailableChallengeItem>>.Success(results);
    }
}