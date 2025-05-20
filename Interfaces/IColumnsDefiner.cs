namespace PDFParserConsoleApp.Interfaces;

public interface IColumnsDefiner
{
    (Dictionary<string, double> Columns, Dictionary<string, double> SaldoColumns) DefineColumns(
        Dictionary<string, (double Left, double Top, double Width, double Height)> specialWords);
}