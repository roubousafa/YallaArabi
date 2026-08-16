using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Reviews.SubmitReview;

public sealed class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Result<SubmitReviewResponse>>
{
    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly IPointsService _pointsService;
    private readonly ICurrentUserService _currentUserService;

    public SubmitReviewCommandHandler(
        IWordReviewRepository wordReviewRepository,
        IPointsService pointsService,
        ICurrentUserService currentUserService)
    {
        _wordReviewRepository = wordReviewRepository;
        _pointsService = pointsService;
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

        var actionKey = request.Grade switch
        {
            ReviewGrade.Easy => "word_reviewed_easy",
            ReviewGrade.Medium => "word_reviewed_medium",
            ReviewGrade.Hard => "word_reviewed_hard",
            _ => "word_reviewed_medium"
        };

        var pointsEarned = await _pointsService.AwardPointsAsync(
           currentUserId.Value,
           actionKey,
           ct: cancellationToken);

        return Result<SubmitReviewResponse>.Success(
            new SubmitReviewResponse(review.WordId, review.IntervalDays, review.NextReviewAt, pointsEarned));
    }
}