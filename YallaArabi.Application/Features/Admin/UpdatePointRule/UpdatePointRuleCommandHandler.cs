using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdatePointRule;

public sealed class UpdatePointRuleCommandHandler : IRequestHandler<UpdatePointRuleCommand, Result<UpdatePointRuleResponse>>
{
    private readonly IPointRuleRepository _pointRuleRepository;

    public UpdatePointRuleCommandHandler(IPointRuleRepository pointRuleRepository)
    {
        _pointRuleRepository = pointRuleRepository;
    }

    public async ValueTask<Result<UpdatePointRuleResponse>> Handle(UpdatePointRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _pointRuleRepository.GetByIdAsync(request.RuleId, cancellationToken);

        if (rule is null)
        {
            return Result<UpdatePointRuleResponse>.Failure("point_rule_not_found", "Point rule not found");
        }

        rule.Update(request.DisplayName, request.Description, request.Points, request.IsActive);
        await _pointRuleRepository.SaveChangesAsync(cancellationToken);

        return Result<UpdatePointRuleResponse>.Success(
            new UpdatePointRuleResponse(rule.Id, rule.ActionKey, rule.Points, rule.IsActive));
    }
}