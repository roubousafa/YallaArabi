using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Challenges.GetChallengeComparison;

public sealed class GetChallengeComparisonQueryHandler : IRequestHandler<GetChallengeComparisonQuery, Result<ChallengeComparisonResponse>>
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetChallengeComparisonQueryHandler(
        IChallengeRepository challengeRepository,
        IChallengeParticipationRepository participationRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _challengeRepository = challengeRepository;
        _participationRepository = participationRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<ChallengeComparisonResponse>> Handle(GetChallengeComparisonQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<ChallengeComparisonResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<ChallengeComparisonResponse>.Failure("challenge_not_found", "Challenge not found");
        }

        var allParticipations = await _participationRepository.GetByChallengeIdAsync(request.ChallengeId, cancellationToken);

        var myParticipation = allParticipations.FirstOrDefault(p => p.UserId == currentUserId.Value && p.IsCompleted());

        if (myParticipation is null)
        {
            return Result<ChallengeComparisonResponse>.Failure("not_completed", "You have not completed this challenge yet");
        }

        var me = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        var myResult = new ParticipantResult(
            currentUserId.Value,
            me?.FullName ?? "-",
            myParticipation.CorrectAnswers,
            myParticipation.TotalQuestions,
            myParticipation.ScorePercentage(),
            myParticipation.PointsEarned,
            myParticipation.CompletedAt);

        var opponentParticipation = allParticipations.FirstOrDefault(p => p.UserId == request.OpponentUserId && p.IsCompleted());

        if (opponentParticipation is null)
        {
            return Result<ChallengeComparisonResponse>.Success(
                new ChallengeComparisonResponse(challenge.Id, challenge.Title, myResult, null, "waiting_for_opponent"));
        }

        var opponentUser = await _userRepository.GetByIdAsync(request.OpponentUserId, cancellationToken);

        var opponentResult = new ParticipantResult(
            request.OpponentUserId,
            opponentUser?.FullName ?? "-",
            opponentParticipation.CorrectAnswers,
            opponentParticipation.TotalQuestions,
            opponentParticipation.ScorePercentage(),
            opponentParticipation.PointsEarned,
            opponentParticipation.CompletedAt);

        var outcome = myParticipation.CorrectAnswers > opponentParticipation.CorrectAnswers
            ? "won"
            : myParticipation.CorrectAnswers < opponentParticipation.CorrectAnswers
                ? "lost"
                : "draw";

        return Result<ChallengeComparisonResponse>.Success(
            new ChallengeComparisonResponse(challenge.Id, challenge.Title, myResult, opponentResult, outcome));
    }
}