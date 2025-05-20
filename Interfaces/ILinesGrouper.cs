using UglyToad.PdfPig.Content;

namespace PDFParserConsoleApp.Interfaces;

public interface ILinesGrouper
{
    List<List<Word>> GroupWordsByLines(List<Word> words, double tolerance);
}