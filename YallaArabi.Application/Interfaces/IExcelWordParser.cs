namespace YallaArabi.Application.Interfaces;

public interface IExcelWordParser
{
    ExcelParseResult Parse(Stream fileStream);
}

public sealed record ExcelWordRow(
    int RowNumber,
    string? LevelCode,
    string? ArabicText,
    string? Transliteration,
    string? EnglishMeaning,
    string? AudioUrl,
    string? ImageUrl
);

public sealed record ExcelParseResult(
    IReadOnlyList<ExcelWordRow> Rows,
    IReadOnlyList<string> ParseErrors
);