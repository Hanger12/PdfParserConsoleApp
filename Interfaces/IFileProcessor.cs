namespace PDFParserConsoleApp.Interfaces;

public interface IFileProcessor
{
    Task ProcessAsync(string folderPath);
}