using Mediator;

namespace YallaArabi.Application.Features.Onboarding;

public sealed class GetSelfAssessmentOptionsQueryHandler : IRequestHandler<GetSelfAssessmentOptionsQuery, IReadOnlyList<SelfAssessmentOption>>
{
    public ValueTask<IReadOnlyList<SelfAssessmentOption>> Handle(GetSelfAssessmentOptionsQuery request, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(SelfAssessmentOptions.All);
    }
}