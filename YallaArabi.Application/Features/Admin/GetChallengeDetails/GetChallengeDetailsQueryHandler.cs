using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetChallengeDetails;

public sealed class GetChallengeDetailsQueryHandler : IRequestHandler<GetChallengeDetailsQuery, Result<ChallengeDetailsResponse>>
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeQuestionRepository _questionRepository;
    private readonly ILevelRepository _levelRepository;

    public GetChallengeDetailsQueryHandler(
        IChallengeRepository challengeRepository,
        IChallengeQuestionRepository questionRepository,
        ILevelRepository levelRepository)
    {
        _challengeRepository = challengeRepository;
        _questionRepository = questionRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<ChallengeDetailsResponse>> Handle(GetChallengeDetailsQuery request, CancellationToken cancellationToken)
    {
        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<ChallengeDetailsResponse>.Failure("challenge_not_found", "Challenge not found");
        }

        string? levelCode = null;

        if (challenge.LevelId.HasValue)
        {
            var level = await _levelRepository.GetByIdAsync(challenge.LevelId.Value, cancellationToken);
            levelCode = level?.Code;
        }

        var questions = await _questionRepository.GetByChallengeIdAsync(challenge.Id, cancellationToken);

        var questionItems = questions
            .Select(q => new ChallengeQuestionItem(
                q.Id,
                q.QuestionText,
                q.OptionA,
                q.OptionB,
                q.OptionC,
                q.OptionD,
                q.CorrectOption,
                q.OrderIndex))
            .ToList();

        var response = new ChallengeDetailsResponse(
            challenge.Id,
            challenge.Title,
            challenge.Description,
            levelCode,
            challenge.PointsReward,
            challenge.TimeLimitSeconds,
            challenge.Status,
            challenge.CreatedAt,
            challenge.ExpiresAt,
            questionItems);

        return Result<ChallengeDetailsResponse>.Success(response);
    }
}