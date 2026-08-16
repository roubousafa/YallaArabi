using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Reviews.SubmitReview;

public sealed record SubmitReviewCommand(Guid WordId, ReviewGrade Grade) : IRequest<Result<SubmitReviewResponse>>;

public sealed record SubmitReviewResponse(Guid WordId, int IntervalDays, DateTime NextReviewAt, int PointsEarned);