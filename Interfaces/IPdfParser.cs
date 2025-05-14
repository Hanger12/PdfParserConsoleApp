using PDFParserApp.Models;

namespace PDFParserApp.Interfaces;

public interface IPdfParser
{
    List<TableRow> Parse(string path);
}
