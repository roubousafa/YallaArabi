using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Onboarding;

public sealed class SetDailyGoalCommandHandler : IRequestHandler<SetDailyGoalCommand, Result<SetDailyGoalResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public SetDailyGoalCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SetDailyGoalResponse>> Handle(SetDailyGoalCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<SetDailyGoalResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<SetDailyGoalResponse>.Failure("user_not_found", "User not found");
        }

        var selectedOption = DailyGoalOptions.All.First(o => o.Key == request.GoalKey);

        user.SetDailyGoal(selectedOption.Minutes);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<SetDailyGoalResponse>.Success(new SetDailyGoalResponse(selectedOption.Key, selectedOption.Minutes));
    }
}