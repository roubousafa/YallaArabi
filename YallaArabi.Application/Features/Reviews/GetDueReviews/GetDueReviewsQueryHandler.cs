using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Reviews.GetDueReviews;

public sealed class GetDueReviewsQueryHandler : IRequestHandler<GetDueReviewsQuery, Result<IReadOnlyList<DueReviewItem>>>
{
    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly IWordRepository _wordRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDueReviewsQueryHandler(
        IWordReviewRepository wordReviewRepository,
        IWordRepository wordRepository,
        ICurrentUserService currentUserService)
    {
        _wordReviewRepository = wordReviewRepository;
        _wordRepository = wordRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<IReadOnlyList<DueReviewItem>>> Handle(GetDueReviewsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<IReadOnlyList<DueReviewItem>>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var dueReviews = await _wordReviewRepository.GetDueReviewsAsync(currentUserId.Value, DateTime.UtcNow, cancellationToken);

        var results = new List<DueReviewItem>();

        foreach (var review in dueReviews)
        {
            var word = await _wordRepository.GetByIdAsync(review.WordId, cancellationToken);

            if (word is not null)
            {
                results.Add(new DueReviewItem(
                    word.Id,
                    word.ArabicText,
                    word.Transliteration,
                    word.EnglishMeaning,
                    word.AudioUrl,
                    word.ImageUrl,
                    review.RepetitionCount));
            }
        }

        return Result<IReadOnlyList<DueReviewItem>>.Success(results);
    }
}