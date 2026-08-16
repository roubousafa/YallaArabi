using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.AddPointRule;

public sealed class AddPointRuleCommandHandler : IRequestHandler<AddPointRuleCommand, Result<AddPointRuleResponse>>
{
    private readonly IPointRuleRepository _pointRuleRepository;

    public AddPointRuleCommandHandler(IPointRuleRepository pointRuleRepository)
    {
        _pointRuleRepository = pointRuleRepository;
    }

    public async ValueTask<Result<AddPointRuleResponse>> Handle(AddPointRuleCommand request, CancellationToken cancellationToken)
    {
        var actionKey = request.ActionKey.Trim().ToLowerInvariant();

        var exists = await _pointRuleRepository.ActionKeyExistsAsync(actionKey, cancellationToken);

        if (exists)
        {
            return Result<AddPointRuleResponse>.Failure("action_key_exists", $"A point rule with action key '{actionKey}' already exists");
        }

        var rule = PointRule.Create(Guid.NewGuid(), actionKey, request.DisplayName.Trim(), request.Description, request.Points);

        await _pointRuleRepository.AddAsync(rule, cancellationToken);
        await _pointRuleRepository.SaveChangesAsync(cancellationToken);

        return Result<AddPointRuleResponse>.Success(new AddPointRuleResponse(rule.Id, rule.ActionKey, rule.Points));
    }
}