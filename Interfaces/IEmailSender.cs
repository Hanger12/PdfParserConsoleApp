namespace PDFParserConsoleApp.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string filePath);
}