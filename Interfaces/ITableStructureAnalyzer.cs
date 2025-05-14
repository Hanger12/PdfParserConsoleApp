using PDFParserApp.Models;

namespace PDFParserApp.Interfaces;

public interface ITableStructureAnalyzer
{
    List<TableRow> Analyze(IReadOnlyList<(string Text, double X, double Y)> words);
}