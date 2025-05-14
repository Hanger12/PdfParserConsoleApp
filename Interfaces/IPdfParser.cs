using PDFParserConsoleApp.Models;

namespace PDFParserConsoleApp.Interfaces;

public interface IPdfParser
{
    List<TableRow> Parse(string path);
}
