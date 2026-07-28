using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Reviews.SubmitReview;

public sealed class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Result<SubmitReviewResponse>>
{
    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly ICurrentUserService _currentUserService;

    public SubmitReviewCommandHandler(
        IWordReviewRepository wordReviewRepository,
        ICurrentUserService currentUserService)
    {
        _wordReviewRepository = wordReviewRepository;
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

        review.ApplyReview(request.Grade);
        await _wordReviewRepository.SaveChangesAsync(cancellationToken);

        return Result<SubmitReviewResponse>.Success(new SubmitReviewResponse(review.WordId, review.IntervalDays, review.NextReviewAt));
    }
}