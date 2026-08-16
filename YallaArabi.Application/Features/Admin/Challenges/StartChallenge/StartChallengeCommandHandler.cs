using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Challenges.StartChallenge;

public sealed class StartChallengeCommandHandler : IRequestHandler<StartChallengeCommand, Result<StartChallengeResponse>>
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeQuestionRepository _questionRepository;
    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly ICurrentUserService _currentUserService;

    public StartChallengeCommandHandler(
        IChallengeRepository challengeRepository,
        IChallengeQuestionRepository questionRepository,
        IChallengeParticipationRepository participationRepository,
        ICurrentUserService currentUserService)
    {
        _challengeRepository = challengeRepository;
        _questionRepository = questionRepository;
        _participationRepository = participationRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<StartChallengeResponse>> Handle(StartChallengeCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<StartChallengeResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<StartChallengeResponse>.Failure("challenge_not_found", "Challenge not found");
        }

        if (!challenge.IsAvailable())
        {
            return Result<StartChallengeResponse>.Failure("challenge_not_available", "This challenge is not available");
        }

        var alreadyCompleted = await _participationRepository.HasCompletedAsync(currentUserId.Value, challenge.Id, cancellationToken);

        if (alreadyCompleted)
        {
            return Result<StartChallengeResponse>.Failure("already_completed", "You have already completed this challenge");
        }

        var questions = await _questionRepository.GetByChallengeIdAsync(challenge.Id, cancellationToken);

        if (questions.Count == 0)
        {
            return Result<StartChallengeResponse>.Failure("no_questions", "This challenge has no questions");
        }

        var existingActive = await _participationRepository.GetActiveAsync(currentUserId.Value, challenge.Id, cancellationToken);

        ChallengeParticipation participation;

        if (existingActive is not null && !existingActive.IsExpired(challenge.TimeLimitSeconds))
        {
            participation = existingActive;
        }
        else
        {
            participation = ChallengeParticipation.Start(challenge.Id, currentUserId.Value, questions.Count);
            await _participationRepository.AddAsync(participation, cancellationToken);
            await _participationRepository.SaveChangesAsync(cancellationToken);
        }

        var questionItems = questions
            .Select(q => new StudentQuestionItem(
                q.Id,
                q.QuestionText,
                q.OptionA,
                q.OptionB,
                q.OptionC,
                q.OptionD,
                q.OrderIndex))
            .ToList();

        var response = new StartChallengeResponse(
            participation.Id,
            challenge.Id,
            challenge.Title,
            challenge.TimeLimitSeconds,
            challenge.PointsReward,
            participation.StartedAt,
            questionItems);

        return Result<StartChallengeResponse>.Success(response);
    }
}