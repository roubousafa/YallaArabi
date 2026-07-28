using Mediator;

namespace YallaArabi.Application.Features.Admin.BulkImportWords;

public sealed record BulkImportWordsCommand(Stream FileStream) : IRequest<BulkImportResult>;

public sealed record BulkImportResult(
    int TotalRowsRead,
    int SuccessfullyImported,
    IReadOnlyList<ImportRowError> Errors
);

public sealed record ImportRowError(int RowNumber, string Reason);