using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Admin.ExportStudentsReport;

namespace YallaArabi.Application.Features.Admin.ExportChallengesReport;

public sealed record ExportChallengesReportQuery(
    Guid? UserId,
    string? LevelCode,
    Guid? ChallengeId
) : IRequest<Result<ExportReportResponse>>;