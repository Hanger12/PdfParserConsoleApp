using System.Globalization;
using ClosedXML.Excel;
using PDFParserConsoleApp.Interfaces;
using PDFParserConsoleApp.Utils;

namespace PDFParserConsoleApp.Services;

public class ExcelExporter : IExcelExporter
{
    private const string DateFormat = "dd.MM.yyyy";
    private const string NumberFormat = "#,##0.00";

    // Новая перегрузка метода SaveToExcel с параметром saldoData
    public void SaveToExcel(List<Dictionary<string, string?>> table, List<string> columnNames, string filePath,
        Dictionary<string, string?>? saldoData = null)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("ExtractedTable");

        WriteHeaders(worksheet, columnNames);
        WriteDataRows(worksheet, table, columnNames);

        if (saldoData != null && saldoData.Count > 0)
        {
            WriteSaldoData(worksheet, saldoData, startColumn: columnNames.Count + 1);
        }

        worksheet.Columns().AdjustToContents();
        workbook.SaveAs(filePath);
    }

    private void WriteHeaders(IXLWorksheet worksheet, List<string> columnNames)
    {
        for (int col = 0; col < columnNames.Count; col++)
        {
            var cell = worksheet.Cell(1, col + 1);
            cell.Value = columnNames[col];
            cell.Style.Font.SetBold();
        }
    }

    private void WriteDataRows(IXLWorksheet worksheet, List<Dictionary<string, string?>> table, List<string> columnNames)
    {
        for (int rowIndex = 0; rowIndex < table.Count; rowIndex++)
        {
            var row = table[rowIndex];
            for (int colIndex = 0; colIndex < columnNames.Count; colIndex++)
            {
                var key = columnNames[colIndex];
                var cell = worksheet.Cell(rowIndex + 2, colIndex + 1);
                var value = row.TryGetValue(key, out var val) ? val : string.Empty;

                SetCellValue(cell, key, value);
            }
        }
    }

    private void SetCellValue(IXLCell cell, string columnName, string? value)
    {
        if (columnName.StartsWith("Дата") && DateTime.TryParse(value, out var date))
        {
            cell.Value = date;
            cell.Style.DateFormat.Format = DateFormat;
        }
        else if (ParsingUtils.TryParseNumber(value, out var number))
        {
            cell.Value = number;
            cell.Style.NumberFormat.Format = NumberFormat;
        }
        else
        {
            cell.Value = value;
        }
    }

    // Метод для фильтрации и записи saldoData в Excel
    private void WriteSaldoData(IXLWorksheet worksheet, Dictionary<string, string?> saldoData, int startColumn)
    {
        // Выбираем ключи с "Сальдо начальное" и сортируем по суффиксу (числу)
        var saldoStartKeys = saldoData.Keys
            .Where(k => k.StartsWith("Сальдо начальное", StringComparison.OrdinalIgnoreCase))
            .OrderBy(k =>
            {
                var suffix = k.Substring("Сальдо начальное".Length);
                return TryParseSuffixIndex(suffix);
            })
            .Take(2)  // берем первые 2
            .ToList();

        // Аналогично для "Сальдо конечное", но берем последние 2
        var saldoEndKeys = saldoData.Keys
            .Where(k => k.StartsWith("Сальдо конечное", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(k =>
            {
                var suffix = k.Substring("Сальдо конечное".Length);
                return TryParseSuffixIndex(suffix);
            })
            .Take(2)  // берем последние 2
            .OrderBy(k =>
            {
                var suffix = k.Substring("Сальдо конечное".Length);
                return TryParseSuffixIndex(suffix);
            }) // сортируем по возрастанию для записи в логичном порядке
            .ToList();

        int currentColum = startColumn;

        void WriteSaldoLine(string key)
        {
            var cellKey = worksheet.Cell(1, currentColum);
            cellKey.Value = key;
            cellKey.Style.Font.SetBold();

            var cellValue = worksheet.Cell(2, currentColum);
            var value = saldoData.TryGetValue(key, out var val) ? val : string.Empty;
            SetCellValue(cellValue, key, value);

            currentColum++;
        }

        foreach (var key in saldoStartKeys)
        {
            WriteSaldoLine(key);
        }

        foreach (var key in saldoEndKeys)
        {
            WriteSaldoLine(key);
        }
    }

    // Метод для парсинга числового суффикса, например "_1", "_2"
    private int TryParseSuffixIndex(string suffix)
    {
        if (string.IsNullOrWhiteSpace(suffix))
            return 0;

        if (suffix.StartsWith("_") && int.TryParse(suffix.Substring(1), out int index))
            return index;

        return int.MaxValue; // если не число, то ставим большой индекс, чтобы в конец
    }
}
