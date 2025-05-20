namespace PDFParserConsoleApp.Interfaces;

public interface IExcelExporter
{

    public void SaveToExcel(
        List<Dictionary<string, string?>> table,
        List<string> columnNames,
        string filePath,
        Dictionary<string, string?>? saldoData = null);
}