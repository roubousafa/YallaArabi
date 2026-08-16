using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetPointRules;

public sealed record GetPointRulesQuery : IRequest<Result<IReadOnlyList<PointRuleItem>>>;

public sealed record PointRuleItem(
    Guid RuleId,
    string ActionKey,
    string DisplayName,
    string? Description,
    int Points,
    bool IsActive,
    DateTime UpdatedAt
);