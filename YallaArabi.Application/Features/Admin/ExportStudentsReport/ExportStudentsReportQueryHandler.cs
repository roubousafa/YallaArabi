using System.Text;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.ExportStudentsReport;

public sealed class ExportStudentsReportQueryHandler : IRequestHandler<ExportStudentsReportQuery, Result<ExportReportResponse>>
{
    private const int MaxExportRecords = 2000;

    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IStudentLessonProgressRepository _progressRepository;
    private readonly ISavedWordRepository _savedWordRepository;
    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IPointsService _pointsService;

    public ExportStudentsReportQueryHandler(
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

    public async ValueTask<Result<ExportReportResponse>> Handle(ExportStudentsReportQuery request, CancellationToken cancellationToken)
    {
        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        Guid? levelId = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = allLevels.FirstOrDefault(l => l.Code == request.LevelCode);

            if (level is null)
            {
                return Result<ExportReportResponse>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
        }

        var students = await _userRepository.GetStudentsReportForExportAsync(
            request.SearchTerm, levelId, request.FromDate, request.ToDate,
            MaxExportRecords, cancellationToken);

        var builder = new StringBuilder();
        builder.AppendLine("FullName,Email,Level,SubscriptionStatus,SubscriptionExpiresAt,TotalPoints,CompletedLessons,SavedWords,CompletedChallenges,RegisteredAt");

        foreach (var user in students)
        {
            var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);
            var isActive = subscription is not null && subscription.IsCurrentlyActive();

            var progress = await _progressRepository.GetByUserIdAsync(user.Id, cancellationToken);
            var savedWords = await _savedWordRepository.GetByUserIdAsync(user.Id, cancellationToken);
            var participations = await _participationRepository.GetByUserIdAsync(user.Id, cancellationToken);
            var points = await _pointsService.GetTotalPointsAsync(user.Id, cancellationToken);

            var levelCode = user.CurrentLevelId.HasValue && levelCodeById.TryGetValue(user.CurrentLevelId.Value, out var code)
                ? code
                : "";

            builder.AppendLine(string.Join(",",
                Escape(user.FullName),
                Escape(user.Email.Value),
                Escape(levelCode),
                isActive ? "premium" : "free",
                isActive && subscription!.ExpiresAt.HasValue ? subscription.ExpiresAt.Value.ToString("yyyy-MM-dd") : "",
                points.ToString(),
                progress.Count.ToString(),
                savedWords.Count.ToString(),
                participations.Count(p => p.IsCompleted()).ToString(),
                user.CreatedAt.ToString("yyyy-MM-dd")));
        }

        var utf8Bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var csvBytes = Encoding.UTF8.GetBytes(builder.ToString());
        var content = utf8Bom.Concat(csvBytes).ToArray();

        var fileName = $"students-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";

        return Result<ExportReportResponse>.Success(
            new ExportReportResponse(content, fileName, students.Count));
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}