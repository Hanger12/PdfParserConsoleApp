using PDFParserConsoleApp.Models;

namespace PDFParserConsoleApp.Interfaces;

public interface IExcelExporter
{
    void Export(List<TableRow> rows, string outputPath);
}