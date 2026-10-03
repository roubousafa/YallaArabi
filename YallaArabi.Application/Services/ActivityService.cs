using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Services;

public sealed class ActivityService : IActivityService
{
    private const string DailyGoalReachedActionKey = "daily_goal_reached";

    private readonly IUserActivityRepository _activityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPointsService _pointsService;
    private readonly IClientTimeZoneProvider _timeZoneProvider;

    public ActivityService(
        IUserActivityRepository activityRepository,
        IUserRepository userRepository,
        IPointsService pointsService,
        IClientTimeZoneProvider timeZoneProvider)
    {
        _activityRepository = activityRepository;
        _userRepository = userRepository;
        _pointsService = pointsService;
        _timeZoneProvider = timeZoneProvider;
    }

    public async Task RecordAsync(Guid userId, string activityType, Guid relatedEntityId, int durationSeconds, CancellationToken ct = default)
    {
        var today = _timeZoneProvider.GetToday();

        // One activity per entity per day, so repeating the same lesson or word doesn't inflate study time.
        var alreadyRecorded = await _activityRepository.ExistsAsync(userId, activityType, relatedEntityId, today, ct);

        if (alreadyRecorded)
        {
            return;
        }

        var activity = UserActivity.Create(userId, activityType, relatedEntityId, today, durationSeconds);
        await _activityRepository.AddAsync(activity, ct);
        await _activityRepository.SaveChangesAsync(ct);

        await AwardDailyGoalIfReachedAsync(userId, today, ct);
    }

    private async Task AwardDailyGoalIfReachedAsync(Guid userId, DateOnly today, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);

        if (user?.DailyGoalMinutes is not int goalMinutes || goalMinutes <= 0)
        {
            return;
        }

        var doneSeconds = await _activityRepository.GetTotalSecondsAsync(userId, today, ct);

        if (doneSeconds < goalMinutes * 60)
        {
            return;
        }

        await _pointsService.AwardPointsAsync(
            userId,
            DailyGoalReachedActionKey,
            DateKey(today),
            today.ToString("yyyy-MM-dd"),
            ct);
    }

    // The date doubles as the related entity id, so PointsService's duplicate check awards the goal once per day.
    private static Guid DateKey(DateOnly date) => new(date.DayNumber, 0, 0, new byte[8]);
}
