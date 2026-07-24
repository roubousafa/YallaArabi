using Mediator;

namespace YallaArabi.Application.Features.Onboarding;

public sealed record GetLearningMotivationOptionsQuery : IRequest<IReadOnlyList<LearningMotivationOption>>;

public sealed class GetLearningMotivationOptionsQueryHandler : IRequestHandler<GetLearningMotivationOptionsQuery, IReadOnlyList<LearningMotivationOption>>
{
    public ValueTask<IReadOnlyList<LearningMotivationOption>> Handle(GetLearningMotivationOptionsQuery request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(LearningMotivationOptions.All);
    }
}