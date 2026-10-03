using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Stats.GetDailyActivity;

public sealed record GetDailyActivityQuery : IRequest<Result<DailyActivityResponse>>;

public sealed record DailyActivityResponse(DateOnly Date, int DoneMinutes, int? GoalMinutes, bool GoalReached);
