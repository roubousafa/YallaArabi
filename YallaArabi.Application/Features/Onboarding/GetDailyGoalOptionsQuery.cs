using Mediator;

namespace YallaArabi.Application.Features.Onboarding;

public sealed record GetDailyGoalOptionsQuery : IRequest<IReadOnlyList<DailyGoalOption>>;

public sealed class GetDailyGoalOptionsQueryHandler : IRequestHandler<GetDailyGoalOptionsQuery, IReadOnlyList<DailyGoalOption>>
{
    public ValueTask<IReadOnlyList<DailyGoalOption>> Handle(GetDailyGoalOptionsQuery request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(DailyGoalOptions.All);
    }
}