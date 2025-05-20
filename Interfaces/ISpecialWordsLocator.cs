using UglyToad.PdfPig.Content;

namespace PDFParserConsoleApp.Interfaces;

public interface ISpecialWordsLocator
{
    Dictionary<string, (double Left, double Top, double Width, double Height)> FindSpecialWords(List<Word> words, List<string> keywords);
}