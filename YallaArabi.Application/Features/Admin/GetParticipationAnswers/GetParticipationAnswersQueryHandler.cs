using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetParticipationAnswers;

public sealed class GetParticipationAnswersQueryHandler : IRequestHandler<GetParticipationAnswersQuery, Result<ParticipationAnswersResponse>>
{
    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IChallengeAnswerRepository _answerRepository;
    private readonly IChallengeQuestionRepository _questionRepository;
    private readonly IChallengeRepository _challengeRepository;
    private readonly IUserRepository _userRepository;

    public GetParticipationAnswersQueryHandler(
        IChallengeParticipationRepository participationRepository,
        IChallengeAnswerRepository answerRepository,
        IChallengeQuestionRepository questionRepository,
        IChallengeRepository challengeRepository,
        IUserRepository userRepository)
    {
        _participationRepository = participationRepository;
        _answerRepository = answerRepository;
        _questionRepository = questionRepository;
        _challengeRepository = challengeRepository;
        _userRepository = userRepository;
    }

    public async ValueTask<Result<ParticipationAnswersResponse>> Handle(GetParticipationAnswersQuery request, CancellationToken cancellationToken)
    {
        var participation = await _participationRepository.GetByIdAsync(request.ParticipationId, cancellationToken);

        if (participation is null)
        {
            return Result<ParticipationAnswersResponse>.Failure("participation_not_found", "Participation not found");
        }

        var user = await _userRepository.GetByIdAsync(participation.UserId, cancellationToken);
        var challenge = await _challengeRepository.GetByIdAsync(participation.ChallengeId, cancellationToken);

        var answers = await _answerRepository.GetByParticipationIdAsync(participation.Id, cancellationToken);
        var questions = await _questionRepository.GetByChallengeIdAsync(participation.ChallengeId, cancellationToken);
        var questionsById = questions.ToDictionary(q => q.Id);

        var items = answers
            .Select(a =>
            {
                questionsById.TryGetValue(a.QuestionId, out var question);

                return new StudentAnswerItem(
                    a.QuestionId,
                    question?.QuestionText ?? "(deleted question)",
                    question?.OptionA,
                    question?.OptionB,
                    question?.OptionC,
                    question?.OptionD,
                    a.SelectedOption,
                    a.CorrectOption,
                    a.IsCorrect,
                    a.OrderIndex);
            })
            .ToList();

        var response = new ParticipationAnswersResponse(
            participation.Id,
            participation.UserId,
            user?.FullName ?? "(deleted user)",
            user?.Email.Value ?? "-",
            participation.ChallengeId,
            challenge?.Title ?? "(deleted challenge)",
            participation.CorrectAnswers,
            participation.TotalQuestions,
            participation.ScorePercentage(),
            participation.PointsEarned,
            participation.IsCompleted() ? "completed" : "in_progress",
            participation.StartedAt,
            participation.CompletedAt,
            items);

        return Result<ParticipationAnswersResponse>.Success(response);
    }
}