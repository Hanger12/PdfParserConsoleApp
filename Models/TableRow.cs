namespace PDFParserApp.Models;

public class TableRow
{
    public string DocumentNumber { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal StartBalance { get; set; }
    public decimal EndBalance { get; set; }
}