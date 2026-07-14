using Mediator;

namespace YallaArabi.Application.Features.Onboarding;

public sealed record GetSelfAssessmentOptionsQuery : IRequest<IReadOnlyList<SelfAssessmentOption>>;