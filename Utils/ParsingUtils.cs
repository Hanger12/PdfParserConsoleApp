using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PDFParserConsoleApp.Utils
{
    public static class ParsingUtils
    {
        public static bool TryParseDate(string text, out string normalizedDate)
        {
            normalizedDate = "";

            if (string.IsNullOrWhiteSpace(text))
                return false;

            var dateText = text.Replace("-", ".").Trim();

            if (DateTime.TryParseExact(dateText, new[]
                    {
                        "dd.MM.yyyy", "dd/MM/yyyy", "dd-MM-yyyy",
                        "d.M.yyyy", "d/M/yyyy", "d-M-yyyy",
                        "yyyy-MM-dd",
                        "dd.MM.yy", "dd/MM/yy", "dd-MM-yy",
                        "d.M.yy", "d/M/yy", "d-M-yy"
                    },
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                normalizedDate = dt.ToString("dd.MM.yyyy");
                return true;
            }

            return false;
        }

        public static bool TryParseNumber(string? value, out double number)
        {
            number = 0;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            // Убираем пробелы (разделители тысяч) и заменяем запятую на точку
            var cleaned = value.Replace(" ", "").Replace(',', '.');

            return double.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out number);
        }

        public static string? ExtractDocumentNumber(string text)
        {
            var match = Regex.Match(text, @"(?:№\s*)?([\d]+[\w/-]*)\s*(?=от|\)|$)", RegexOptions.IgnoreCase);
            return match.Success ? "№" + match.Groups[1].Value.Trim() : null;
        }
    }
}