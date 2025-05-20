using PDFParserConsoleApp.Interfaces;
using UglyToad.PdfPig.Content;

namespace PDFParserConsoleApp.Services;

public class SpecialWordsLocator : ISpecialWordsLocator
{
    public Dictionary<string, (double Left, double Top, double Width, double Height)> FindSpecialWords(List<Word> words, List<string> keywords)
    {
        var specialWords = new Dictionary<string, (double Left, double Top, double Width, double Height)>(StringComparer.OrdinalIgnoreCase);
        var counters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // Преобразуем ключевые слова в список списков слов (для составных фраз)
        var keywordTokens = keywords
            .Select(k => new { Original = k, Tokens = k.Split(' ', StringSplitOptions.RemoveEmptyEntries) })
            .ToList();

        for (int i = 0; i < words.Count; i++)
        {
            foreach (var keyword in keywordTokens)
            {
                if (i + keyword.Tokens.Length > words.Count)
                    continue;

                bool match = true;
                for (int j = 0; j < keyword.Tokens.Length; j++)
                {
                    if (!string.Equals(words[i + j].Text.Trim(), keyword.Tokens[j], StringComparison.OrdinalIgnoreCase))
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    string key = keyword.Original;
                    if (!counters.ContainsKey(key))
                        counters[key] = 0;

                    while (specialWords.ContainsKey(key))
                    {
                        counters[key]++;
                        key = $"{keyword.Original}_{counters[key]}";
                    }

                    // Получаем bounding box по всей фразе
                    var firstBox = words[i].BoundingBox;
                    var lastBox = words[i + keyword.Tokens.Length - 1].BoundingBox;

                    double left = firstBox.Left;
                    double top = firstBox.Top;
                    double width = (lastBox.Left + lastBox.Width) - firstBox.Left;
                    double height = Math.Max(firstBox.Height, lastBox.Height); 

                    specialWords[key] = (left, top, width, height);

                    Console.WriteLine(
                        $"Найдено ключевое слово: '{key}' на позиции: Left={left:0.###}, Top={top:0.###} width={width:0.###}, Height={height:0.###}");

                    i += keyword.Tokens.Length - 1; // продвигаемся вперёд
                    break; // чтобы не искать одновременно вложенные ключи
                }
            }
        }

        Console.WriteLine($"Найдено ключевых слов: {specialWords.Count}");
        return specialWords;
    }
}
