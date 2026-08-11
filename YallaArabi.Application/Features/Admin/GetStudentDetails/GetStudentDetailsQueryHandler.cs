using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetStudentDetails;

public sealed class GetStudentDetailsQueryHandler : IRequestHandler<GetStudentDetailsQuery, Result<StudentDetailsResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IStudentLessonProgressRepository _progressRepository;
    private readonly ISavedWordRepository _savedWordRepository;
    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IWordRepository _wordRepository;

    public GetStudentDetailsQueryHandler(
        IUserRepository userRepository,
        ILevelRepository levelRepository,
        ISubscriptionRepository subscriptionRepository,
        IStudentLessonProgressRepository progressRepository,
        ISavedWordRepository savedWordRepository,
        IWordReviewRepository wordReviewRepository,
        ILessonRepository lessonRepository,
        IWordRepository wordRepository)
    {
        _userRepository = userRepository;
        _levelRepository = levelRepository;
        _subscriptionRepository = subscriptionRepository;
        _progressRepository = progressRepository;
        _savedWordRepository = savedWordRepository;
        _wordReviewRepository = wordReviewRepository;
        _lessonRepository = lessonRepository;
        _wordRepository = wordRepository;
    }

    public async ValueTask<Result<StudentDetailsResponse>> Handle(GetStudentDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result<StudentDetailsResponse>.Failure("user_not_found", "User not found");
        }

        string? levelCode = null;
        string? levelName = null;

        if (user.CurrentLevelId.HasValue)
        {
            var level = await _levelRepository.GetByIdAsync(user.CurrentLevelId.Value, cancellationToken);
            levelCode = level?.Code;
            levelName = level?.Name;
        }

        var activeSubscription = await _subscriptionRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);
        var isSubscriptionActive = activeSubscription is not null && activeSubscription.IsCurrentlyActive();

        var subscriptionInfo = new StudentSubscriptionInfo(
            isSubscriptionActive ? "premium" : "free",
            isSubscriptionActive ? activeSubscription!.PlanCode : null,
            isSubscriptionActive ? activeSubscription!.StartedAt : null,
            isSubscriptionActive ? activeSubscription!.ExpiresAt : null);

        var completedProgress = await _progressRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var totalPoints = 0;

        foreach (var progress in completedProgress)
        {
            var lesson = await _lessonRepository.GetByIdAsync(progress.LessonId, cancellationToken);
            if (lesson is not null)
            {
                totalPoints += lesson.PointsReward;
            }
        }

        var savedWords = await _savedWordRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var dueReviews = await _wordReviewRepository.GetDueReviewsAsync(request.UserId, DateTime.UtcNow.AddYears(100), cancellationToken);

        var savedWordItems = new List<StudentSavedWordItem>();

        foreach (var saved in savedWords)
        {
            var word = await _wordRepository.GetByIdAsync(saved.WordId, cancellationToken);

            if (word is not null)
            {
                savedWordItems.Add(new StudentSavedWordItem(
                    word.Id,
                    word.ArabicText,
                    word.Transliteration,
                    word.EnglishMeaning,
                    saved.SavedAt));
            }
        }

        var progressInfo = new StudentProgressInfo(
            completedProgress.Count,
            totalPoints,
            savedWords.Count,
            dueReviews.Count);

        var response = new StudentDetailsResponse(
            user.Id,
            user.FullName,
            user.Email.Value,
            user.Role,
            levelCode,
            levelName,
            user.CreatedAt,
            subscriptionInfo,
            progressInfo,
            savedWordItems);

        return Result<StudentDetailsResponse>.Success(response);
    }
}