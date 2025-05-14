using PDFParserApp.Interfaces;
using PDFParserApp.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PDFParserApp.Services;

public class PdfParser : IPdfParser
{
    private readonly ITableStructureAnalyzer _analyzer;

    public PdfParser(ITableStructureAnalyzer analyzer)
    {
        _analyzer = analyzer;
    }
    public List<TableRow> Parse(string path)
    {
        var allWords = new List<(string Text, double X, double Y)>();

        using var document = PdfDocument.Open(path);
        foreach (var page in document.GetPages())
        {
            var words = page.GetWords()
                .Select(w => (w.Text, w.BoundingBox.Left, w.BoundingBox.Top))
                .ToList();
            allWords.AddRange(words);
        }

        return _analyzer.Analyze(allWords);
    }
}