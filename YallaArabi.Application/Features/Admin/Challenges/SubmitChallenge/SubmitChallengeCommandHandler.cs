using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Challenges.SubmitChallenge;

public sealed class SubmitChallengeCommandHandler : IRequestHandler<SubmitChallengeCommand, Result<SubmitChallengeResponse>>
{
    private const string ChallengeWonActionKey = "challenge_won";
    private const string ChallengeParticipatedActionKey = "challenge_participated";
    private const int WinThresholdPercentage = 60;

    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeQuestionRepository _questionRepository;
    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IChallengeAnswerRepository _answerRepository;
    private readonly IPointsService _pointsService;
    private readonly ICurrentUserService _currentUserService;

    public SubmitChallengeCommandHandler(
        IChallengeRepository challengeRepository,
        IChallengeQuestionRepository questionRepository,
        IChallengeParticipationRepository participationRepository,
        IChallengeAnswerRepository answerRepository,
        IPointsService pointsService,
        ICurrentUserService currentUserService)
    {
        _challengeRepository = challengeRepository;
        _questionRepository = questionRepository;
        _participationRepository = participationRepository;
        _answerRepository = answerRepository;
        _pointsService = pointsService;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SubmitChallengeResponse>> Handle(SubmitChallengeCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<SubmitChallengeResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken);

        if (participation is null)
        {
            return Result<SubmitChallengeResponse>.Failure("participation_not_found", "Challenge participation not found");
        }

        if (participation.UserId != currentUserId.Value)
        {
            return Result<SubmitChallengeResponse>.Failure("forbidden", "This participation does not belong to you");
        }

        if (participation.IsCompleted())
        {
            return Result<SubmitChallengeResponse>.Failure("already_submitted", "You have already submitted this challenge");
        }

        var challenge = await _challengeRepository.GetByIdAsync(participation.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<SubmitChallengeResponse>.Failure("challenge_not_found", "Challenge not found");
        }

        var isExpired = participation.IsExpired(challenge.TimeLimitSeconds);

        var questions = await _questionRepository.GetByChallengeIdAsync(challenge.Id, cancellationToken);

        var results = new List<AnswerResult>();
        var answersToSave = new List<ChallengeAnswer>();
        var correctCount = 0;

        foreach (var question in questions)
        {
            var submitted = request.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
            var selectedOption = submitted?.SelectedOption?.Trim().ToUpperInvariant() ?? "-";

            var isCorrect = !isExpired && question.IsCorrectAnswer(selectedOption);

            if (isCorrect)
            {
                correctCount++;
            }

            results.Add(new AnswerResult(
                question.Id,
                question.QuestionText,
                selectedOption,
                question.CorrectOption,
                isCorrect));

            answersToSave.Add(ChallengeAnswer.Create(
                participation.Id,
                question.Id,
                submitted?.SelectedOption?.Trim().ToUpperInvariant(),
                question.CorrectOption,
                isCorrect,
                question.OrderIndex));
        }

        var scorePercentage = questions.Count == 0 ? 0 : (int)Math.Round(correctCount * 100.0 / questions.Count);

        var actionKey = scorePercentage >= WinThresholdPercentage
            ? ChallengeWonActionKey
            : ChallengeParticipatedActionKey;

        var pointsEarned = await _pointsService.AwardPointsAsync(
            currentUserId.Value,
            actionKey,
            challenge.Id,
            ct: cancellationToken);

        await _answerRepository.AddRangeAsync(answersToSave, cancellationToken);
        await _answerRepository.SaveChangesAsync(cancellationToken);

        participation.Complete(correctCount, pointsEarned);
        await _participationRepository.SaveChangesAsync(cancellationToken);

        var response = new SubmitChallengeResponse(
            participation.Id,
            questions.Count,
            correctCount,
            scorePercentage,
            pointsEarned,
            results);

        return Result<SubmitChallengeResponse>.Success(response);
    }
}