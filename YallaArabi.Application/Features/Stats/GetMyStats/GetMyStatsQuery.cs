using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Stats.GetMyStats;

public sealed record GetMyStatsQuery : IRequest<Result<MyStatsResponse>>;

public sealed record MyStatsResponse(int Streak, int Xp, bool IsActiveToday);
