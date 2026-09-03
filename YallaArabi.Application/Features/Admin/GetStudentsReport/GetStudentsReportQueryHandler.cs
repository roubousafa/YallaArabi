using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetStudentsReport;

public sealed class GetStudentsReportQueryHandler : IRequestHandler<GetStudentsReportQuery, Result<PagedStudentsReportResponse>>
{
    private const int MaxPageSize = 100;

    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IStudentLessonProgressRepository _progressRepository;
    private readonly ISavedWordRepository _savedWordRepository;
    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IPointsService _pointsService;

    public GetStudentsReportQueryHandler(
        IUserRepository userRepository,
        ILevelRepository levelRepository,
        ISubscriptionRepository subscriptionRepository,
        IStudentLessonProgressRepository progressRepository,
        ISavedWordRepository savedWordRepository,
        IChallengeParticipationRepository participationRepository,
        IPointsService pointsService)
    {
        _userRepository = userRepository;
        _levelRepository = levelRepository;
        _subscriptionRepository = subscriptionRepository;
        _progressRepository = progressRepository;
        _savedWordRepository = savedWordRepository;
        _participationRepository = participationRepository;
        _pointsService = pointsService;
    }

    public async ValueTask<Result<PagedStudentsReportResponse>> Handle(GetStudentsReportQuery request, CancellationToken cancellationToken)
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
                return Result<PagedStudentsReportResponse>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
        }

        var (items, totalCount) = await _userRepository.GetStudentsReportAsync(
            request.SearchTerm, levelId, request.FromDate, request.ToDate,
            pageNumber, pageSize, cancellationToken);

        var results = new List<StudentReportItem>();

        foreach (var user in items)
        {
            var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);
            var isActive = subscription is not null && subscription.IsCurrentlyActive();

            var progress = await _progressRepository.GetByUserIdAsync(user.Id, cancellationToken);
            var savedWords = await _savedWordRepository.GetByUserIdAsync(user.Id, cancellationToken);
            var participations = await _participationRepository.GetByUserIdAsync(user.Id, cancellationToken);
            var points = await _pointsService.GetTotalPointsAsync(user.Id, cancellationToken);

            results.Add(new StudentReportItem(
                user.Id,
                user.FullName,
                user.Email.Value,
                user.CurrentLevelId.HasValue && levelCodeById.TryGetValue(user.CurrentLevelId.Value, out var code) ? code : null,
                isActive ? "premium" : "free",
                isActive ? subscription!.ExpiresAt : null,
                points,
                progress.Count,
                savedWords.Count,
                participations.Count(p => p.IsCompleted()),
                user.CreatedAt));
        }

        var summary = new StudentsReportSummary(
            totalCount,
            results.Count(r => r.SubscriptionStatus == "premium"),
            results.Count(r => r.SubscriptionStatus == "free"),
            results.Sum(r => r.TotalPoints));

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return Result<PagedStudentsReportResponse>.Success(
            new PagedStudentsReportResponse(results, totalCount, pageNumber, pageSize, totalPages, summary));
    }
}