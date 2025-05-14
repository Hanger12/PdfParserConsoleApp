namespace PDFParserApp.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string filePath);
}