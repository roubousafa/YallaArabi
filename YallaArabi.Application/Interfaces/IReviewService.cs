using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewOutcome> SubmitAsync(Guid userId, WordReview review, ReviewGrade grade, CancellationToken ct = default);
}

public sealed record ReviewOutcome(int PointsEarned, bool WasDue);
