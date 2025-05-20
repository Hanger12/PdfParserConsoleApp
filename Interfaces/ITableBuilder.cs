namespace PDFParserConsoleApp.Interfaces;

public interface ITableBuilder
{
    List<Dictionary<string, string?>> BuildTable(Dictionary<string, List<(string? Text, double Top)>> columnData);
}