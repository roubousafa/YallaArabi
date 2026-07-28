using FluentValidation;

namespace YallaArabi.Application.Features.Subscriptions.CreateCheckout;

public sealed class CreateCheckoutCommandValidator : AbstractValidator<CreateCheckoutCommand>
{
    private static readonly string[] ValidPlanCodes = { "monthly", "yearly" };

    public CreateCheckoutCommandValidator()
    {
        RuleFor(x => x.PlanCode)
            .NotEmpty().WithMessage("Plan code is required")
            .Must(code => ValidPlanCodes.Contains(code))
            .WithMessage("Invalid plan code. Valid options: monthly, yearly");
    }
}