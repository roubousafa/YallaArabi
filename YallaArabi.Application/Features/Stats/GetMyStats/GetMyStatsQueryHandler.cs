using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Stats.GetMyStats;

public sealed class GetMyStatsQueryHandler : IRequestHandler<GetMyStatsQuery, Result<MyStatsResponse>>
{
    private readonly IUserActivityRepository _activityRepository;
    private readonly IPointsService _pointsService;
    private readonly IClientTimeZoneProvider _timeZoneProvider;
    private readonly ICurrentUserService _currentUserService;

    public GetMyStatsQueryHandler(
        IUserActivityRepository activityRepository,
        IPointsService pointsService,
        IClientTimeZoneProvider timeZoneProvider,
        ICurrentUserService currentUserService)
    {
        _activityRepository = activityRepository;
        _pointsService = pointsService;
        _timeZoneProvider = timeZoneProvider;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<MyStatsResponse>> Handle(GetMyStatsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<MyStatsResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var today = _timeZoneProvider.GetToday();
        var activeDates = await _activityRepository.GetActiveDatesAsync(currentUserId.Value, cancellationToken);
        var xp = await _pointsService.GetTotalPointsAsync(currentUserId.Value, cancellationToken);

        var isActiveToday = activeDates.Contains(today);
        var streak = CalculateStreak(activeDates, today, isActiveToday);

        return Result<MyStatsResponse>.Success(new MyStatsResponse(streak, xp, isActiveToday));
    }

    // Counts consecutive active days ending today, or ending yesterday if the student hasn't studied yet today,
    // so the streak isn't shown as broken before the day is over.
    private static int CalculateStreak(IReadOnlyList<DateOnly> activeDates, DateOnly today, bool isActiveToday)
    {
        var activeSet = activeDates.ToHashSet();
        var day = isActiveToday ? today : today.AddDays(-1);
        var streak = 0;

        while (activeSet.Contains(day))
        {
            streak++;
            day = day.AddDays(-1);
        }

        return streak;
    }
}
