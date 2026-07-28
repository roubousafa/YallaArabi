using Mediator;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.BulkImportWords;

public sealed class BulkImportWordsCommandHandler : IRequestHandler<BulkImportWordsCommand, BulkImportResult>
{
    private const int MaxRowsPerImport = 1000;

    private readonly IExcelWordParser _parser;
    private readonly ILevelRepository _levelRepository;
    private readonly IWordRepository _wordRepository;

    public BulkImportWordsCommandHandler(
        IExcelWordParser parser,
        ILevelRepository levelRepository,
        IWordRepository wordRepository)
    {
        _parser = parser;
        _levelRepository = levelRepository;
        _wordRepository = wordRepository;
    }

    public async ValueTask<BulkImportResult> Handle(BulkImportWordsCommand request, CancellationToken cancellationToken)
    {
        var parseResult = _parser.Parse(request.FileStream);
        var errors = new List<ImportRowError>();

        if (parseResult.ParseErrors.Count > 0)
        {
            foreach (var parseError in parseResult.ParseErrors)
            {
                errors.Add(new ImportRowError(0, parseError));
            }

            return new BulkImportResult(0, 0, errors);
        }

        if (parseResult.Rows.Count > MaxRowsPerImport)
        {
            errors.Add(new ImportRowError(0, $"File contains {parseResult.Rows.Count} rows, which exceeds the maximum of {MaxRowsPerImport} rows per import."));
            return new BulkImportResult(parseResult.Rows.Count, 0, errors);
        }

        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelsByCode = allLevels.ToDictionary(l => l.Code, l => l);

        var successCount = 0;

        foreach (var row in parseResult.Rows)
        {
            var rowError = ValidateRow(row, levelsByCode, out var level);

            if (rowError is not null)
            {
                errors.Add(new ImportRowError(row.RowNumber, rowError));
                continue;
            }

            var word = Word.Create(
                level!.Id,
                row.ArabicText!,
                row.Transliteration!,
                row.EnglishMeaning!,
                row.AudioUrl,
                row.ImageUrl);

            await _wordRepository.AddAsync(word, cancellationToken);
            successCount++;
        }

        if (successCount > 0)
        {
            await _wordRepository.SaveChangesAsync(cancellationToken);
        }

        return new BulkImportResult(parseResult.Rows.Count, successCount, errors);
    }

    private static string? ValidateRow(
        Application.Interfaces.ExcelWordRow row,
        Dictionary<string, Level> levelsByCode,
        out Level? level)
    {
        level = null;

        if (string.IsNullOrWhiteSpace(row.LevelCode))
            return "Level code is missing";

        if (!levelsByCode.TryGetValue(row.LevelCode, out var foundLevel))
            return $"Level code '{row.LevelCode}' does not exist";

        level = foundLevel;

        if (string.IsNullOrWhiteSpace(row.ArabicText))
            return "Arabic text is missing";

        if (string.IsNullOrWhiteSpace(row.Transliteration))
            return "Transliteration is missing";

        if (string.IsNullOrWhiteSpace(row.EnglishMeaning))
            return "English meaning is missing";

        return null;
    }
}