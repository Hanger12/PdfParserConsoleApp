using PDFParserConsoleApp.Interfaces;
using PDFParserConsoleApp.Models;

namespace PDFParserConsoleApp.Services;

public class TableStructureAnalyzer : ITableStructureAnalyzer
{
    public List<TableRow> Analyze(IReadOnlyList<(string Text, double X, double Y)> words)
    {
        var rows = new List<TableRow>();

        var groupedByLine = words
            .GroupBy(w => Math.Round(w.Y, 1))
            .OrderByDescending(g => g.Key)
            .Select(g => g.OrderBy(w => w.X).ToList())
            .ToList();
        
        foreach (var line in groupedByLine)
        {
            // Тестовая реализация для проверки работоспособности 
            if (line.Count < 6) continue;

            var row = new TableRow();
            try
            {
                row.DocumentNumber = line[0].Text;
                row.Date = DateTime.TryParse(line[1].Text, out var d) ? d : DateTime.MinValue;
                row.Debit = decimal.TryParse(line[2].Text.Replace(" ", ""), out var debit) ? debit : 0;
                row.Credit = decimal.TryParse(line[3].Text.Replace(" ", ""), out var credit) ? credit : 0;
                row.StartBalance = decimal.TryParse(line[4].Text.Replace(" ", ""), out var sb) ? sb : 0;
                row.EndBalance = decimal.TryParse(line[5].Text.Replace(" ", ""), out var eb) ? eb : 0;
                rows.Add(row);
            }
            catch { }
        }

        return rows;
    }
}