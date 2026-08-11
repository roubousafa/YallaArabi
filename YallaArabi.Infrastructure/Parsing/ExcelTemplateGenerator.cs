using ClosedXML.Excel;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Parsing;

public class ExcelTemplateGenerator : IExcelTemplateGenerator
{
    public byte[] GenerateWordsTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Words");

        worksheet.Cell(1, 1).Value = "LevelCode";
        worksheet.Cell(1, 2).Value = "ArabicText";
        worksheet.Cell(1, 3).Value = "Transliteration";
        worksheet.Cell(1, 4).Value = "EnglishMeaning";
        worksheet.Cell(1, 5).Value = "AudioUrl";
        worksheet.Cell(1, 6).Value = "ImageUrl";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

        worksheet.Cell(2, 1).Value = "A0";
        worksheet.Cell(2, 2).Value = "مرحبا";
        worksheet.Cell(2, 3).Value = "mar7aba";
        worksheet.Cell(2, 4).Value = "Hello";
        worksheet.Cell(2, 5).Value = "";
        worksheet.Cell(2, 6).Value = "";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}