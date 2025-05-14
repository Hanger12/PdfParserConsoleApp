using PDFParserConsoleApp.Models;

namespace PDFParserConsoleApp.Interfaces;

public interface ITableStructureAnalyzer
{
    List<TableRow> Analyze(IReadOnlyList<(string Text, double X, double Y)> words);
}