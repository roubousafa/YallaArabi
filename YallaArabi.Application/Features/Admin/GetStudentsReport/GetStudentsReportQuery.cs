using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetStudentsReport;

public sealed record GetStudentsReportQuery(
    string? SearchTerm,
    string? LevelCode,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedStudentsReportResponse>>;

public sealed record PagedStudentsReportResponse(
    IReadOnlyList<StudentReportItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    StudentsReportSummary Summary
);

public sealed record StudentReportItem(
    Guid UserId,
    string FullName,
    string Email,
    string? LevelCode,
    string SubscriptionStatus,
    DateTime? SubscriptionExpiresAt,
    int TotalPoints,
    int CompletedLessons,
    int SavedWords,
    int CompletedChallenges,
    DateTime RegisteredAt
);

public sealed record StudentsReportSummary(
    int TotalStudents,
    int PremiumCount,
    int FreeCount,
    int TotalPointsSum
);