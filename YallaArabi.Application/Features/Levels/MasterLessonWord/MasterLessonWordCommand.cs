using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Levels.MasterLessonWord;

public sealed record MasterLessonWordCommand(Guid WordId, ReviewGrade Grade) : IRequest<Result<MasterLessonWordResponse>>;

public sealed record MasterLessonWordResponse(
    Guid WordId,
    string Mastery,
    int IntervalDays,
    DateTime NextReviewAt,
    int PointsEarned,
    bool WasDue,
    bool AddedToReviews
);

public sealed class MasterLessonWordCommandHandler : IRequestHandler<MasterLessonWordCommand, Result<MasterLessonWordResponse>>
{
    private readonly IWordRepository _wordRepository;
    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly IReviewService _reviewService;
    private readonly ICurrentUserService _currentUserService;

    public MasterLessonWordCommandHandler(
        IWordRepository wordRepository,
        IWordReviewRepository wordReviewRepository,
        IReviewService reviewService,
        ICurrentUserService currentUserService)
    {
        _wordRepository = wordRepository;
        _wordReviewRepository = wordReviewRepository;
        _reviewService = reviewService;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<MasterLessonWordResponse>> Handle(MasterLessonWordCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<MasterLessonWordResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        if (await _wordRepository.GetByIdAsync(request.WordId, cancellationToken) is null)
        {
            return Result<MasterLessonWordResponse>.Failure("word_not_found", "Word not found");
        }

        // Practising a word in a lesson enrolls it in the student's SRS deck, so mastery is tracked by the review system.
        var review = await _wordReviewRepository.GetAsync(currentUserId.Value, request.WordId, cancellationToken);
        var addedToReviews = review is null;

        if (review is null)
        {
            review = WordReview.StartReviewing(currentUserId.Value, request.WordId);
            await _wordReviewRepository.AddAsync(review, cancellationToken);
            await _wordReviewRepository.SaveChangesAsync(cancellationToken);
        }

        var outcome = await _reviewService.SubmitAsync(currentUserId.Value, review, request.Grade, cancellationToken);

        return Result<MasterLessonWordResponse>.Success(new MasterLessonWordResponse(
            review.WordId,
            WordMastery.Of(review),
            review.IntervalDays,
            review.NextReviewAt,
            outcome.PointsEarned,
            outcome.WasDue,
            addedToReviews));
    }
}
