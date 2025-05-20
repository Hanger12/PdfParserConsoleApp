using PDFParserConsoleApp.Interfaces;
using PdfDocument = UglyToad.PdfPig.PdfDocument;

namespace PDFParserConsoleApp.Services;

public class PdfParser : IPdfParser
{
    private Dictionary<string, (double Left, double Top, double Width, double Height)> _globalSpecialWords
        = new(StringComparer.OrdinalIgnoreCase);
    
    public (List<Dictionary<string, string?>> Rows, List<string> ColumnNames, Dictionary<string, string?> SaldoData) Parse(string path)
    {
        var specialWordsLocator = new SpecialWordsLocator();
        var linesGrouper = new LinesGrouper();
        var columnsDefiner = new ColumnsDefiner();
        var columnExtractor = new ColumnDataExtractor();
        var tableBuilder = new TableBuilder();

        var keywords = new List<string> { "Дата", "Документ", "Дебет", "Кредит", "Номер", "Сальдо начальное", "Сальдо конечное" };
        Dictionary<string, string?> saldoDataCombined = new();
        var allRows = new List<Dictionary<string, string?>>();
        var columnNames = new List<string>();

        using var document = PdfDocument.Open(path);
        var allSpecialWords = new Dictionary<string, (double Left, double Top, double Width, double Height)>(StringComparer.OrdinalIgnoreCase);
        foreach (var page in document.GetPages())
        {
            var words = page.GetWords().ToList();

            var pageSpecialWords = specialWordsLocator.FindSpecialWords(words, keywords);

            // Добавляем в общий список, если ещё не было ключа
            foreach (var kvp in pageSpecialWords)
            {
                allSpecialWords[kvp.Key] = kvp.Value;
            }

            var (columns, saldoColumns) = columnsDefiner.DefineColumns(allSpecialWords);
            var lines = linesGrouper.GroupWordsByLines(words, 3);
            var columnData = columnExtractor.ExtractColumns(lines, columns, page.Number == 1, allSpecialWords);
            var saldoData = columnExtractor.ExtractSaldoValues(lines, saldoColumns, allSpecialWords);
            // Сохраняем имена колонок только один раз — с первой непустой страницы
            if (columnNames.Count == 0 && columnData.Count > 0)
            {
                columnNames = columnData.Keys.ToList();
            }

            foreach (var saldo in saldoData)
            {
                saldoDataCombined[saldo.Key] = saldo.Value;
            }

            foreach (var saldo in saldoDataCombined)
            {
                Console.WriteLine($"{saldo.Key}: {saldo.Value}");
            }
            var parsedTable = tableBuilder.BuildTable(columnData);
            Console.WriteLine("\nСобранная таблица:");
            Console.WriteLine(string.Join(" | ", columnNames));
            foreach (var row in parsedTable)
            {
                var values = columnNames.Select(name => row.TryGetValue(name, out var val) ? val ?? "NULL" : "NULL");
                Console.WriteLine(string.Join(" | ", values));
            }

            allRows.AddRange(parsedTable);
        }

        return (allRows, columnNames, saldoDataCombined);
    }
}