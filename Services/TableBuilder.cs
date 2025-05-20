using PDFParserConsoleApp.Interfaces;

namespace PDFParserConsoleApp.Services;

public class TableBuilder : ITableBuilder
{
    public List<Dictionary<string, string?>> BuildTable(Dictionary<string, List<(string? Text, double Top)>> columnData)
    {
        var table = new List<Dictionary<string, string?>>();
        // Собираем все уникальные top-координаты (строки)
        var allTops = new HashSet<double>();
        foreach (var col in columnData.Values)
        {
            foreach (var entry in col)
                allTops.Add(entry.Top);
        }

        var sortedTops = allTops.OrderByDescending(t => t).ToList();

        foreach (var top in sortedTops)
        {
            var row = new Dictionary<string, string?>();

            foreach (var colName in columnData.Keys)
            {
                var val = columnData[colName].FirstOrDefault(c => Math.Abs(c.Top - top) < 1e-6);
                row[colName] = val.Text;
            }

            // Добавляем только если хоть одна ячейка не пустая
            if (row.Values.Any(v => !string.IsNullOrWhiteSpace(v)))
                table.Add(row);
        }

        return table;
    }
}