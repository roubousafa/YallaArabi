using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.ExportStudentsReport;

public sealed record ExportStudentsReportQuery(
    string? SearchTerm,
    string? LevelCode,
    DateTime? FromDate,
    DateTime? ToDate
) : IRequest<Result<ExportReportResponse>>;

public sealed record ExportReportResponse(byte[] Content, string FileName, int RecordsCount);