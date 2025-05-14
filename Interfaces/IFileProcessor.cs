namespace PDFParserApp.Interfaces;

public interface IFileProcessor
{
    Task ProcessAsync(string folderPath);
}