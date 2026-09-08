using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetChallengeParticipants;

public sealed class GetChallengeParticipantsQueryHandler : IRequestHandler<GetChallengeParticipantsQuery, Result<ChallengeParticipantsResponse>>
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeQuestionRepository _questionRepository;
    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;

    public GetChallengeParticipantsQueryHandler(
        IChallengeRepository challengeRepository,
        IChallengeQuestionRepository questionRepository,
        IChallengeParticipationRepository participationRepository,
        IUserRepository userRepository,
        ILevelRepository levelRepository)
    {
        _challengeRepository = challengeRepository;
        _questionRepository = questionRepository;
        _participationRepository = participationRepository;
        _userRepository = userRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<ChallengeParticipantsResponse>> Handle(GetChallengeParticipantsQuery request, CancellationToken cancellationToken)
    {
        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<ChallengeParticipantsResponse>.Failure("challenge_not_found", "Challenge not found");
        }

        var questionsCount = await _questionRepository.CountByChallengeIdAsync(challenge.Id, cancellationToken);
        var participations = await _participationRepository.GetByChallengeIdAsync(challenge.Id, cancellationToken);

        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        var ordered = participations
            .OrderByDescending(p => p.IsCompleted())
            .ThenByDescending(p => p.CorrectAnswers)
            .ThenBy(p => p.CompletedAt ?? DateTime.MaxValue)
            .ToList();

        var participants = new List<ParticipantItem>();
        var rank = 0;

        foreach (var participation in ordered)
        {
            var user = await _userRepository.GetByIdAsync(participation.UserId, cancellationToken);

            rank++;

            participants.Add(new ParticipantItem(
                participation.Id,
                participation.UserId,
                user?.FullName ?? "(deleted user)",
                user?.Email.Value ?? "-",
                user?.CurrentLevelId is not null && levelCodeById.TryGetValue(user.CurrentLevelId.Value, out var code) ? code : null,
                participation.CorrectAnswers,
                participation.TotalQuestions,
                participation.ScorePercentage(),
                participation.PointsEarned,
                participation.IsCompleted() ? "completed" : "in_progress",
                participation.StartedAt,
                participation.CompletedAt,
                participation.IsCompleted() ? rank : 0));
        }

        var completed = participants.Where(p => p.Status == "completed").ToList();
        var averageScore = completed.Count == 0
            ? 0
            : Math.Round(completed.Average(p => p.ScorePercentage), 1);

        var response = new ChallengeParticipantsResponse(
            challenge.Id,
            challenge.Title,
            questionsCount,
            participants.Count,
            completed.Count,
            averageScore,
            participants);

        return Result<ChallengeParticipantsResponse>.Success(response);
    }
}