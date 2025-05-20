using PDFParserConsoleApp.Interfaces;
using UglyToad.PdfPig.Content;

namespace PDFParserConsoleApp.Services;

public class LinesGrouper : ILinesGrouper
{
    public List<List<Word>> GroupWordsByLines(List<Word> words, double tolerance)
    {
        var lines = new List<List<Word>>();
        var sortedWords = words.OrderByDescending(w => w.BoundingBox.Top).ToList();

        List<Word> currentLine = new();
        double? currentLineTop = null;

        foreach (var word in sortedWords)
        {
            if (currentLineTop == null)
            {
                currentLineTop = word.BoundingBox.Top;
                currentLine.Add(word);
            }
            else if (Math.Abs(word.BoundingBox.Top - currentLineTop.Value) <= tolerance)
            {
                currentLine.Add(word);
            }
            else
            {
                lines.Add(currentLine.OrderBy(w => w.BoundingBox.Left).ToList());
                currentLine = new List<Word> { word };
                currentLineTop = word.BoundingBox.Top;
            }
        }

        if (currentLine.Count > 0)
            lines.Add(currentLine.OrderBy(w => w.BoundingBox.Left).ToList());

        return lines;
    }
}