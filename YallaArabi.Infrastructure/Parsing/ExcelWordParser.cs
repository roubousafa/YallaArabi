using ClosedXML.Excel;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Parsing;

public class ExcelWordParser : IExcelWordParser
{
    private const int HeaderRowNumber = 1;
    private const int FirstDataRowNumber = 2;

    public ExcelParseResult Parse(Stream fileStream)
    {
        var rows = new List<ExcelWordRow>();
        var errors = new List<string>();

        try
        {
            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheet(1);
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? HeaderRowNumber;

            for (var rowNumber = FirstDataRowNumber; rowNumber <= lastRow; rowNumber++)
            {
                var row = worksheet.Row(rowNumber);

                if (row.IsEmpty())
                {
                    continue;
                }

                rows.Add(new ExcelWordRow(
                    RowNumber: rowNumber,
                    LevelCode: GetCellValue(row, 1),
                    ArabicText: GetCellValue(row, 2),
                    Transliteration: GetCellValue(row, 3),
                    EnglishMeaning: GetCellValue(row, 4),
                    AudioUrl: GetCellValue(row, 5),
                    ImageUrl: GetCellValue(row, 6)
                ));
            }
        }
        catch (Exception ex)
        {
            errors.Add($"Failed to read the Excel file: {ex.Message}");
        }

        return new ExcelParseResult(rows, errors);
    }

    private static string? GetCellValue(IXLRow row, int columnNumber)
    {
        var value = row.Cell(columnNumber).GetString().Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}