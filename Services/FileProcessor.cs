using PDFParserApp.Interfaces;
using PDFParserApp.Models;

namespace PDFParserApp.Services;

public class FileProcessor : IFileProcessor
{
    private readonly IPdfParser _parser;
    private readonly IExcelExporter _exporter;
    private readonly IEmailSender? _sender;

    public FileProcessor(IPdfParser parser, IExcelExporter exporter, IEmailSender? sender)
    {
        _parser = parser;
        _exporter = exporter;
        _sender = sender;
    }
    public async Task ProcessAsync(string folderPath)
    {
        var files = Directory.GetFiles(folderPath, "*.pdf");
        foreach (var file in files)
        {
            Console.WriteLine($"Processing file: {file}");
            var rows = _parser.Parse(file);
            var outputPath = Path.ChangeExtension(file, ".xlsx");
            _exporter.Export(rows, outputPath);

            if (_sender != null)
                await _sender.SendAsync(outputPath);
        }
    }
}