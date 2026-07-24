using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Onboarding;

public sealed record SetDailyGoalCommand(string GoalKey) : IRequest<Result<SetDailyGoalResponse>>;

public sealed record SetDailyGoalResponse(string GoalKey, int Minutes);