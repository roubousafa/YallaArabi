using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetChallengesReport;

public sealed class GetChallengesReportQueryHandler : IRequestHandler<GetChallengesReportQuery, Result<PagedChallengesReportResponse>>
{
    private const int MaxPageSize = 100;

    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IChallengeRepository _challengeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;

    public GetChallengesReportQueryHandler(
        IChallengeParticipationRepository participationRepository,
        IChallengeRepository challengeRepository,
        IUserRepository userRepository,
        ILevelRepository levelRepository)
    {
        _participationRepository = participationRepository;
        _challengeRepository = challengeRepository;
        _userRepository = userRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<PagedChallengesReportResponse>> Handle(GetChallengesReportQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, MaxPageSize);

        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        IReadOnlyList<Guid>? challengeIds = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = allLevels.FirstOrDefault(l => l.Code == request.LevelCode);

            if (level is null)
            {
                return Result<PagedChallengesReportResponse>.Failure("level_not_found", "Level not found");
            }

            var (levelChallenges, _) = await _challengeRepository.GetPagedAsync(
                null, level.Id, 1, 1000, cancellationToken);

            challengeIds = levelChallenges.Select(c => c.Id).ToList();

            if (challengeIds.Count == 0)
            {
                var emptySummary = new ChallengesReportSummary(0, 0, 0, 0);
                return Result<PagedChallengesReportResponse>.Success(
                    new PagedChallengesReportResponse(new List<ChallengeReportItem>(), 0, pageNumber, pageSize, 0, emptySummary));
            }
        }

        var (items, totalCount) = await _participationRepository.GetReportAsync(
            request.UserId, request.ChallengeId, challengeIds, pageNumber, pageSize, cancellationToken);

        var results = new List<ChallengeReportItem>();

        foreach (var participation in items)
        {
            var user = await _userRepository.GetByIdAsync(participation.UserId, cancellationToken);
            var challenge = await _challengeRepository.GetByIdAsync(participation.ChallengeId, cancellationToken);

            results.Add(new ChallengeReportItem(
                participation.Id,
                participation.UserId,
                user?.FullName ?? "(deleted user)",
                user?.Email.Value ?? "-",
                user?.CurrentLevelId is not null && levelCodeById.TryGetValue(user.CurrentLevelId.Value, out var studentLevel) ? studentLevel : null,
                participation.ChallengeId,
                challenge?.Title ?? "(deleted challenge)",
                challenge?.LevelId is not null && levelCodeById.TryGetValue(challenge.LevelId.Value, out var challengeLevel) ? challengeLevel : null,
                participation.CorrectAnswers,
                participation.TotalQuestions,
                participation.ScorePercentage(),
                participation.PointsEarned,
                participation.IsCompleted() ? "completed" : "in_progress",
                participation.StartedAt,
                participation.CompletedAt));
        }

        var completed = results.Where(r => r.Status == "completed").ToList();

        var summary = new ChallengesReportSummary(
            totalCount,
            completed.Count,
            results.Count - completed.Count,
            completed.Count == 0 ? 0 : Math.Round(completed.Average(r => r.ScorePercentage), 1));

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result<PagedChallengesReportResponse>.Success(
            new PagedChallengesReportResponse(results, totalCount, pageNumber, pageSize, totalPages, summary));
    }
}