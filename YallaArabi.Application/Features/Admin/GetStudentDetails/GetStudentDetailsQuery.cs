using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetStudentDetails;

public sealed record GetStudentDetailsQuery(Guid UserId) : IRequest<Result<StudentDetailsResponse>>;

public sealed record StudentDetailsResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    string? LevelCode,
    string? LevelName,
     string? PreferredDialect,
    DateTime CreatedAt,
    StudentSubscriptionInfo Subscription,
    StudentProgressInfo Progress,
    IReadOnlyList<StudentSavedWordItem> SavedWords
);

public sealed record StudentSubscriptionInfo(
    string Status,
    string? PlanCode,
    DateTime? StartedAt,
    DateTime? ExpiresAt
);

public sealed record StudentProgressInfo(
    int CompletedLessonsCount,
    int TotalPoints,
    int SavedWordsCount,
    int WordsInReviewCount
);

public sealed record StudentSavedWordItem(
    Guid WordId,
    string ArabicText,
    string Transliteration,
    string EnglishMeaning,
    DateTime SavedAt
);