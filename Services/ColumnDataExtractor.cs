using System.Globalization;
using System.Text.RegularExpressions;
using PDFParserConsoleApp.Interfaces;
using PDFParserConsoleApp.Utils;
using UglyToad.PdfPig.Content;

namespace PDFParserConsoleApp.Services;

public class ColumnDataExtractor : IColumnDataExtractor
{
    public Dictionary<string, List<(string? Text, double Top)>> ExtractColumns(
        List<List<Word>> lines,
        Dictionary<string, double> columns,
        bool isFirstPage,
        Dictionary<string, (double Left, double Top, double Width, double Height)> specialWords)
    {
        var result = new Dictionary<string, List<(string Text, double Top)>>();

        var headerTops = new Dictionary<string, double>();
        if (isFirstPage)
        {
            foreach (var kvp in columns)
            {
                if (specialWords.TryGetValue(kvp.Key, out var headerPos))
                    headerTops[kvp.Key] = headerPos.Top;
            }
        }


        // Обычное извлечение колонок
        foreach (var kvp in columns)
        {
            string columnName = kvp.Key;
            double columnLeft = kvp.Value;
            double xTolerance = columnName.StartsWith("Документ", StringComparison.OrdinalIgnoreCase) ? 80.0 : 22.0;

            var columnResults = new List<(string Text, double Top)>();
            const double yTolerance = 12;
            foreach (var line in lines)
            {
                double avgTop = line.Average(w => w.BoundingBox.Top);

                if (isFirstPage &&
                    headerTops.TryGetValue(columnName, out var headerTop) &&
                    avgTop >= headerTop - yTolerance)
                    continue;

                var wordsInColumn = line
                    .Where(w => Math.Abs(w.BoundingBox.Left - columnLeft) <= xTolerance)
                    .ToList();

                if (wordsInColumn.Count == 0)
                    continue;

                var text = string.Join(" ", wordsInColumn.Select(w => w.Text));
                columnResults.Add((text, avgTop));
            }

            result[columnName] = columnResults;
        }

        NormalizeDateColumn(result);
        FilterByDateTops(result);
        CleanDocumentColumn(result);

        return result;
    }

    public Dictionary<string, string> ExtractSaldoValues(
        List<List<Word>> lines,
        Dictionary<string, double> saldoColumns,
        Dictionary<string, (double Left, double Top, double Width, double Height)> specialWords
    )
    {
        var result = new Dictionary<string, string>();

        const double yTolerance = 1.5; // допуск по вертикали
        double xOffset = 20.0; // смещение вправо от правой границы заголовка
        double captureWidth = 190.0; // ширина зоны захвата вправо
        foreach (var columnName in saldoColumns.Keys)
        {
            if (!specialWords.TryGetValue(columnName, out var headerPos))
                continue;

            double targetTop = headerPos.Top;
            double startLeft = headerPos.Left + headerPos.Width + xOffset;
            double endLeft = startLeft + captureWidth;

            var matchingLines = lines
                .Where(line => line.Any(w => Math.Abs(w.BoundingBox.Top - targetTop) <= yTolerance))
                .ToList();

            if (matchingLines.Count == 0)
                continue;

            string extracted = null;

            foreach (var line in matchingLines)
            {
                var rightWords = line
                    .Where(w =>
                        w.BoundingBox.Left >= startLeft &&
                        w.BoundingBox.Left <= endLeft &&
                        Math.Abs(w.BoundingBox.Top - targetTop) <= yTolerance)
                    .OrderBy(w => w.BoundingBox.Left)
                    .ToList();

                if (rightWords.Count == 0)
                    continue;

                extracted  = string.Join(" ", rightWords.Select(w => w.Text.Trim()));
            }

            if (extracted != null)
                result[columnName] = extracted;
        }
        return result;
    }


    private void FilterByDateTops(Dictionary<string, List<(string Text, double Top)>> columns)
    {
        if (!columns.TryGetValue("Дата", out var dateEntries))
            return;

        var validTops = dateEntries.Select(d => d.Top).ToHashSet();

        foreach (var key in columns.Keys.ToList())
        {
            var filtered = columns[key].Where(c => validTops.Contains(c.Top)).ToList();
            columns[key] = filtered;
        }
    }

    private void NormalizeDateColumn(Dictionary<string, List<(string Text, double Top)>> columns)
    {
        if (!columns.TryGetValue("Дата", out var dateEntries))
            return;

        var validDateEntries = new List<(string Text, double Top)>();

        foreach (var (text, top) in dateEntries)
        {
            if (ParsingUtils.TryParseDate(text, out var normalizedDate))
            {
                validDateEntries.Add((normalizedDate, top));
            }
        }

        columns["Дата"] = validDateEntries;
    }

    private void CleanDocumentColumn(Dictionary<string, List<(string Text, double Top)>> columns)
    {
        foreach (var columnName in columns.Keys.ToList())
        {
            if (!columnName.StartsWith("Документ", StringComparison.OrdinalIgnoreCase))
                continue;

            var cleaned = new List<(string Text, double Top)>();

            foreach (var (text, top) in columns[columnName])
            {
                var processedText = ParsingUtils.ExtractDocumentNumber(text);
                cleaned.Add((processedText ?? text, top));
            }

            columns[columnName] = cleaned;
        }
    }
}