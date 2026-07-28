using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Reviews.GetDueReviews;

public sealed record GetDueReviewsQuery : IRequest<Result<IReadOnlyList<DueReviewItem>>>;

public sealed record DueReviewItem(
    Guid WordId,
    string ArabicText,
    string Transliteration,
    string EnglishMeaning,
    string? AudioUrl,
    string? ImageUrl,
    int RepetitionCount
);