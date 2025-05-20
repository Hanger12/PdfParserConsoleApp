using PDFParserConsoleApp.Interfaces;

namespace PDFParserConsoleApp.Services;

public class ColumnsDefiner : IColumnsDefiner
{
    public (Dictionary<string, double> Columns, Dictionary<string, double> SaldoColumns) DefineColumns(
        Dictionary<string, (double Left, double Top, double Width, double Height)> specialWords)
    {
        var columns = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var saldoColumns = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in specialWords)
        {
            if (kvp.Key.StartsWith("Сальдо начальное", StringComparison.OrdinalIgnoreCase) ||
                kvp.Key.StartsWith("Сальдо конечное", StringComparison.OrdinalIgnoreCase))
            {
                saldoColumns[kvp.Key] = kvp.Value.Left;
            }
            else
            {
                columns[kvp.Key] = kvp.Value.Left;
            }
        }

        Console.WriteLine("Определены основные столбцы:");
        foreach (var col in columns)
            Console.WriteLine($"  {col.Key} => Left = {col.Value:0.###}");

        Console.WriteLine("Определены столбцы сальдо:");
        foreach (var saldo in saldoColumns)
            Console.WriteLine($"  {saldo.Key} => Left = {saldo.Value:0.###}");

        return (columns, saldoColumns);
    }
}