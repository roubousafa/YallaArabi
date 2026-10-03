using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Stats.GetDailyActivity;

public sealed class GetDailyActivityQueryHandler : IRequestHandler<GetDailyActivityQuery, Result<DailyActivityResponse>>
{
    private readonly IUserActivityRepository _activityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClientTimeZoneProvider _timeZoneProvider;
    private readonly ICurrentUserService _currentUserService;

    public GetDailyActivityQueryHandler(
        IUserActivityRepository activityRepository,
        IUserRepository userRepository,
        IClientTimeZoneProvider timeZoneProvider,
        ICurrentUserService currentUserService)
    {
        _activityRepository = activityRepository;
        _userRepository = userRepository;
        _timeZoneProvider = timeZoneProvider;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<DailyActivityResponse>> Handle(GetDailyActivityQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<DailyActivityResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<DailyActivityResponse>.Failure("user_not_found", "User not found");
        }

        var today = _timeZoneProvider.GetToday();
        var doneSeconds = await _activityRepository.GetTotalSecondsAsync(user.Id, today, cancellationToken);
        var goalMinutes = user.DailyGoalMinutes;
        var goalReached = goalMinutes > 0 && doneSeconds >= goalMinutes * 60;

        return Result<DailyActivityResponse>.Success(
            new DailyActivityResponse(today, doneSeconds / 60, goalMinutes, goalReached));
    }
}
