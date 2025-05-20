namespace PDFParserConsoleApp.Interfaces;

public interface IPdfParser
{
    public (List<Dictionary<string, string?>> Rows, List<string> ColumnNames, Dictionary<string, string?> SaldoData) Parse(string path);
}