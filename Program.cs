using PDFParserApp.Services;

namespace PDFParserApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var folderPath = args.FirstOrDefault(a => a.StartsWith("--folderPath="))?.Split("=")[1];
        var sendEmail = args.Any(a => a == "--sendEmail");

        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
        {
            Console.WriteLine("Укажите корректный путь через аргумент: --folderPath=ПУТЬ");
            return;
        }

        var processor = new FileProcessor(
            new PdfParser(new TableStructureAnalyzer()),
            new ExcelExporter(),
            null);

        await processor.ProcessAsync(folderPath);
    }
}