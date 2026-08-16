using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdatePointRule;

public sealed record UpdatePointRuleCommand(
    Guid RuleId,
    string DisplayName,
    string? Description,
    int Points,
    bool IsActive
) : IRequest<Result<UpdatePointRuleResponse>>;

public sealed record UpdatePointRuleResponse(Guid RuleId, string ActionKey, int Points, bool IsActive);