using PDFParserApp.Interfaces;
using PDFParserApp.Models;
using ClosedXML.Excel;
namespace PDFParserApp.Services;

public class ExcelExporter : IExcelExporter
{
    public void Export(List<TableRow> rows, string outputPath)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");

        var headers = new[] { "№ документа", "Дата", "Дебет", "Кредит", "Нач. сальдо", "Кон. сальдо" };
        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
        }

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            worksheet.Cell(i + 2, 1).Value = row.DocumentNumber;
            worksheet.Cell(i + 2, 2).Value = row.Date;
            worksheet.Cell(i + 2, 2).Style.DateFormat.Format = "dd.MM.yyyy";
            worksheet.Cell(i + 2, 3).Value = row.Debit;
            worksheet.Cell(i + 2, 4).Value = row.Credit;
            worksheet.Cell(i + 2, 5).Value = row.StartBalance;
            worksheet.Cell(i + 2, 6).Value = row.EndBalance;

            for (int j = 3; j <= 6; j++)
            {
                worksheet.Cell(i + 2, j).Style.NumberFormat.Format = "#,##0.00";
            }
        }

        worksheet.Columns().AdjustToContents();
        workbook.SaveAs(outputPath);
    }
}