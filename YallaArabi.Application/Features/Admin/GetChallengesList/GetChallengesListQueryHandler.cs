using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetChallengesList;

public sealed class GetChallengesListQueryHandler : IRequestHandler<GetChallengesListQuery, Result<PagedChallengesResponse>>
{
    private const int MaxPageSize = 100;

    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeQuestionRepository _questionRepository;
    private readonly ILevelRepository _levelRepository;

    public GetChallengesListQueryHandler(
        IChallengeRepository challengeRepository,
        IChallengeQuestionRepository questionRepository,
        ILevelRepository levelRepository)
    {
        _challengeRepository = challengeRepository;
        _questionRepository = questionRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<PagedChallengesResponse>> Handle(GetChallengesListQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, MaxPageSize);

        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        Guid? levelId = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = allLevels.FirstOrDefault(l => l.Code == request.LevelCode);

            if (level is null)
            {
                return Result<PagedChallengesResponse>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
        }

        var (items, totalCount) = await _challengeRepository.GetPagedAsync(
            request.Status, levelId, pageNumber, pageSize, cancellationToken);

        var results = new List<AdminChallengeItem>();

        foreach (var challenge in items)
        {
            var questionsCount = await _questionRepository.CountByChallengeIdAsync(challenge.Id, cancellationToken);

            results.Add(new AdminChallengeItem(
                challenge.Id,
                challenge.Title,
                challenge.Description,
                challenge.LevelId.HasValue && levelCodeById.TryGetValue(challenge.LevelId.Value, out var code) ? code : null,
                challenge.PointsReward,
                challenge.TimeLimitSeconds,
                challenge.Status,
                questionsCount,
                challenge.CreatedAt,
                challenge.ExpiresAt));
        }

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result<PagedChallengesResponse>.Success(
            new PagedChallengesResponse(results, totalCount, pageNumber, pageSize, totalPages));
    }
}