using InvoiceExtractor.Models;
using PDFtoImage;
using System.IO;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace InvoiceExtractor.Services
{
    public class PdfProcessingService : IPdfProcessingService
    {
        // Extracts invoice details using PdfPig for text extraction
        public InvoiceModel ExtractInvoice(string pdfPath, TemplateModel template)
        {
            string fullText = ExtractFullTextFromPdf(pdfPath);
            if (string.IsNullOrEmpty(fullText)) return null!;

            InvoiceModel invoice = new InvoiceModel();
            foreach (var field in template.Fields.Values)
            {
                string value = ExtractFieldValue(fullText, field.Keyword);
                if (!string.IsNullOrEmpty(value))
                {
                    AssignValueToInvoice(invoice, field.FieldName, value);
                }
            }

            return invoice;
        }

        // Check if template matches the PDF content using PdfPig
        public bool IsTemplateMatch(string pdfPath, TemplateModel template)
        {
            string fullText = ExtractFullTextFromPdf(pdfPath);
            if (string.IsNullOrEmpty(fullText)) return false;

            foreach (var field in template.Fields.Values)
            {
                if (!fullText.Contains(field.Keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }

        // Convert PDF page to image using PDFtoImage
        public string ConvertPdfPageToImage(string pdfPath, int pageNumber = 1, int dpi = 300)
        {
            string outputImagePath = Path.Combine(Path.GetDirectoryName(pdfPath), $"pdf_page_{pageNumber}.png");

            try
            {
                using (var pdfStream = File.OpenRead(pdfPath))
                {
                    // Using PDFtoImage library to convert PDF to image
                    Conversion.SavePng(outputImagePath, pdfStream, (Index)(pageNumber - 1), options: new(Dpi: dpi));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting PDF to image: {ex.Message}");
                return null!;
            }

            return outputImagePath;
        }

        // Extract full text from the PDF using PdfPig
        private string ExtractFullTextFromPdf(string pdfPath)
        {
            try
            {
                using (var document = PdfDocument.Open(pdfPath))
                {
                    var fullText = new System.Text.StringBuilder();

                    // Use a more advanced text extraction method to keep word separation intact
                    foreach (var page in document.GetPages())
                    {
                        var text = ContentOrderTextExtractor.GetText(page);
                        fullText.AppendLine(text);
                    }

                    return fullText.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text from PDF: {ex.Message}");
                return null!;
            }
        }

        // Extract field value using regex
        private string ExtractFieldValue(string fullText, string keyword)
        {
            string pattern = $"{Regex.Escape(keyword)}\\s*(.*)";
            var match = Regex.Match(fullText, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }

        // Assign extracted values to InvoiceModel fields
        private void AssignValueToInvoice(InvoiceModel invoice, string fieldName, string value)
        {
            switch (fieldName)
            {
                case "InvoiceNumber":
                    invoice.InvoiceNumber = value;
                    break;
                case "InvoiceDate":
                    if (DateTime.TryParse(value, out DateTime date))
                        invoice.InvoiceDate = date.ToString("yyyy-MM-dd");
                    break;
                case "Vendor":
                    invoice.Vendor = value;
                    break;
                case "Description":
                    invoice.Description = value;
                    break;
                case "Amount":
                    if (decimal.TryParse(value, out decimal amount))
                        invoice.Amount = amount;
                    break;
            }
        }
    }
}
