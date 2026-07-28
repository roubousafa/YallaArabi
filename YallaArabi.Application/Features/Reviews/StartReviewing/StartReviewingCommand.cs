using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Reviews.StartReviewing;

public sealed record StartReviewingCommand : IRequest<Result<StartReviewingResponse>>;

public sealed record StartReviewingResponse(int WordsAdded, int AlreadyReviewing);