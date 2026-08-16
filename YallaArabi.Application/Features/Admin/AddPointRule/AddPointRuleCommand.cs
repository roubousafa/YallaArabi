using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.AddPointRule;

public sealed record AddPointRuleCommand(
    string ActionKey,
    string DisplayName,
    string? Description,
    int Points
) : IRequest<Result<AddPointRuleResponse>>;

public sealed record AddPointRuleResponse(Guid RuleId, string ActionKey, int Points);