using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Reviews.StartReviewing;

public sealed class StartReviewingCommandHandler : IRequestHandler<StartReviewingCommand, Result<StartReviewingResponse>>
{
    private readonly ISavedWordRepository _savedWordRepository;
    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly ICurrentUserService _currentUserService;

    public StartReviewingCommandHandler(
        ISavedWordRepository savedWordRepository,
        IWordReviewRepository wordReviewRepository,
        ICurrentUserService currentUserService)
    {
        _savedWordRepository = savedWordRepository;
        _wordReviewRepository = wordReviewRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<StartReviewingResponse>> Handle(StartReviewingCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<StartReviewingResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var savedWords = await _savedWordRepository.GetByUserIdAsync(currentUserId.Value, cancellationToken);

        var wordsAdded = 0;
        var alreadyReviewing = 0;

        foreach (var saved in savedWords)
        {
            var exists = await _wordReviewRepository.ExistsAsync(currentUserId.Value, saved.WordId, cancellationToken);

            if (exists)
            {
                alreadyReviewing++;
                continue;
            }

            var review = WordReview.StartReviewing(currentUserId.Value, saved.WordId);
            await _wordReviewRepository.AddAsync(review, cancellationToken);
            wordsAdded++;
        }

        await _wordReviewRepository.SaveChangesAsync(cancellationToken);

        return Result<StartReviewingResponse>.Success(new StartReviewingResponse(wordsAdded, alreadyReviewing));
    }
}