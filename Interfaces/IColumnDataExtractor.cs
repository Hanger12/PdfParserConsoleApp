using UglyToad.PdfPig.Content;

namespace PDFParserConsoleApp.Interfaces;

public interface IColumnDataExtractor
{
    Dictionary<string, List<(string? Text, double Top)>> ExtractColumns(List<List<Word>> lines, Dictionary<string, double> columns, bool isFirstPage, Dictionary<string, (double Left, double Top, double Width, double Height)> specialWords);
}