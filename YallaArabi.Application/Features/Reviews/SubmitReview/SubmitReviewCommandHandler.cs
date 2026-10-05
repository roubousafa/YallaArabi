using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Reviews.SubmitReview;

public sealed class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Result<SubmitReviewResponse>>
{
    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly IReviewService _reviewService;
    private readonly ICurrentUserService _currentUserService;

    public SubmitReviewCommandHandler(
        IWordReviewRepository wordReviewRepository,
        IReviewService reviewService,
        ICurrentUserService currentUserService)
    {
        _wordReviewRepository = wordReviewRepository;
        _reviewService = reviewService;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SubmitReviewResponse>> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<SubmitReviewResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var review = await _wordReviewRepository.GetAsync(currentUserId.Value, request.WordId, cancellationToken);

        if (review is null)
        {
            return Result<SubmitReviewResponse>.Failure("review_not_found", "This word is not in your review list yet");
        }

        var outcome = await _reviewService.SubmitAsync(currentUserId.Value, review, request.Grade, cancellationToken);

        return Result<SubmitReviewResponse>.Success(
            new SubmitReviewResponse(review.WordId, review.IntervalDays, review.NextReviewAt, outcome.PointsEarned, outcome.WasDue));
    }
}
