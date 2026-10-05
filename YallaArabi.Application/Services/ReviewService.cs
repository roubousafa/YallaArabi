using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Services;

// Shared by the review screen and lesson word practice so both follow the same SRS, points and activity rules.
public sealed class ReviewService : IReviewService
{
    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly IPointsService _pointsService;
    private readonly IActivityService _activityService;

    public ReviewService(
        IWordReviewRepository wordReviewRepository,
        IPointsService pointsService,
        IActivityService activityService)
    {
        _wordReviewRepository = wordReviewRepository;
        _pointsService = pointsService;
        _activityService = activityService;
    }

    public async Task<ReviewOutcome> SubmitAsync(Guid userId, WordReview review, ReviewGrade grade, CancellationToken ct = default)
    {
        // Early (not yet due) reviews count as practice only: they don't move the SRS schedule or earn points,
        // so resubmitting the same word can neither farm XP nor push it weeks ahead.
        var wasDue = review.IsDue();
        var pointsEarned = 0;

        if (wasDue)
        {
            review.ApplyReview(grade);
            await _wordReviewRepository.SaveChangesAsync(ct);

            var actionKey = grade switch
            {
                ReviewGrade.Easy => "word_reviewed_easy",
                ReviewGrade.Medium => "word_reviewed_medium",
                ReviewGrade.Hard => "word_reviewed_hard",
                _ => "word_reviewed_medium"
            };

            pointsEarned = await _pointsService.AwardPointsAsync(userId, actionKey, ct: ct);
        }

        await _activityService.RecordAsync(
            userId,
            ActivityTypes.WordReviewed,
            review.WordId,
            ActivityTypes.ReviewEstimatedSeconds,
            ct);

        return new ReviewOutcome(pointsEarned, wasDue);
    }
}
