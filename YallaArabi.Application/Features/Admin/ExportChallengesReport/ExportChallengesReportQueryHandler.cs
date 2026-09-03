using System.Text;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Admin.ExportStudentsReport;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.ExportChallengesReport;

public sealed class ExportChallengesReportQueryHandler : IRequestHandler<ExportChallengesReportQuery, Result<ExportReportResponse>>
{
    private const int MaxExportRecords = 5000;

    private readonly IChallengeParticipationRepository _participationRepository;
    private readonly IChallengeRepository _challengeRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;

    public ExportChallengesReportQueryHandler(
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

    public async ValueTask<Result<ExportReportResponse>> Handle(ExportChallengesReportQuery request, CancellationToken cancellationToken)
    {
        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        IReadOnlyList<Guid>? challengeIds = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = allLevels.FirstOrDefault(l => l.Code == request.LevelCode);

            if (level is null)
            {
                return Result<ExportReportResponse>.Failure("level_not_found", "Level not found");
            }

            var (levelChallenges, _) = await _challengeRepository.GetPagedAsync(
                null, level.Id, 1, 1000, cancellationToken);

            challengeIds = levelChallenges.Select(c => c.Id).ToList();
        }

        var participations = await _participationRepository.GetReportForExportAsync(
            request.UserId, request.ChallengeId, challengeIds, MaxExportRecords, cancellationToken);

        var builder = new StringBuilder();
        builder.AppendLine("StudentName,StudentEmail,StudentLevel,ChallengeTitle,ChallengeLevel,CorrectAnswers,TotalQuestions,ScorePercentage,PointsEarned,Status,StartedAt,CompletedAt");

        foreach (var participation in participations)
        {
            var user = await _userRepository.GetByIdAsync(participation.UserId, cancellationToken);
            var challenge = await _challengeRepository.GetByIdAsync(participation.ChallengeId, cancellationToken);

            var studentLevel = user?.CurrentLevelId is not null && levelCodeById.TryGetValue(user.CurrentLevelId.Value, out var sl) ? sl : "";
            var challengeLevel = challenge?.LevelId is not null && levelCodeById.TryGetValue(challenge.LevelId.Value, out var cl) ? cl : "";

            builder.AppendLine(string.Join(",",
                Escape(user?.FullName ?? "(deleted user)"),
                Escape(user?.Email.Value ?? ""),
                Escape(studentLevel),
                Escape(challenge?.Title ?? "(deleted challenge)"),
                Escape(challengeLevel),
                participation.CorrectAnswers.ToString(),
                participation.TotalQuestions.ToString(),
                participation.ScorePercentage().ToString(),
                participation.PointsEarned.ToString(),
                participation.IsCompleted() ? "completed" : "in_progress",
                participation.StartedAt.ToString("yyyy-MM-dd HH:mm"),
                participation.CompletedAt?.ToString("yyyy-MM-dd HH:mm") ?? ""));
        }

        var utf8Bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var csvBytes = Encoding.UTF8.GetBytes(builder.ToString());
        var content = utf8Bom.Concat(csvBytes).ToArray();

        var fileName = $"challenges-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";

        return Result<ExportReportResponse>.Success(
            new ExportReportResponse(content, fileName, participations.Count));
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