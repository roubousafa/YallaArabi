using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetPointRules;

public sealed class GetPointRulesQueryHandler : IRequestHandler<GetPointRulesQuery, Result<IReadOnlyList<PointRuleItem>>>
{
    private readonly IPointRuleRepository _pointRuleRepository;

    public GetPointRulesQueryHandler(IPointRuleRepository pointRuleRepository)
    {
        _pointRuleRepository = pointRuleRepository;
    }

    public async ValueTask<Result<IReadOnlyList<PointRuleItem>>> Handle(GetPointRulesQuery request, CancellationToken cancellationToken)
    {
        var rules = await _pointRuleRepository.GetAllAsync(cancellationToken);

        var results = rules
            .Select(r => new PointRuleItem(
                r.Id,
                r.ActionKey,
                r.DisplayName,
                r.Description,
                r.Points,
                r.IsActive,
                r.UpdatedAt))
            .ToList();

        return Result<IReadOnlyList<PointRuleItem>>.Success(results);
    }
}