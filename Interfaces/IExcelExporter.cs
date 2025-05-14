using PDFParserApp.Models;

namespace PDFParserApp.Interfaces;

public interface IExcelExporter
{
    void Export(List<TableRow> rows, string outputPath);
}