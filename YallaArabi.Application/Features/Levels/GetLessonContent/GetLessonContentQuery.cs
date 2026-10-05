using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Words.SearchWords;

namespace YallaArabi.Application.Features.Levels.GetLessonContent;

public sealed record GetLessonContentQuery(Guid LessonId) : IRequest<Result<LessonContentResponse>>;

public sealed record LessonContentResponse(
    Guid LessonId,
    string Title,
    string? Description,
    string? IconUrl,
    int PointsReward,
    bool IsCompleted,
    IReadOnlyList<WordSearchResult> Words,
    IReadOnlyList<WordProgressItem> WordProgress,
    IReadOnlyList<LessonQuestionItem> Questions
);

public sealed record WordProgressItem(Guid WordId, string Mastery, DateTime? NextReviewAt);

public sealed record LessonQuestionItem(
    Guid QuestionId,
    string Type,
    string? QuestionText,
    IReadOnlyList<QuestionOption> Options,
    string? CorrectOption,
    WordSearchResult? Word,
    string? AudioUrl,
    string? ImageUrl,
    int OrderIndex
);

public sealed record QuestionOption(string Key, string Text);
