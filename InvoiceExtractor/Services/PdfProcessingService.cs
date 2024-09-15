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
        // Extracts multiple invoice details from a single PDF using PdfPig for text extraction
        public List<InvoiceModel> ExtractInvoices(string pdfPath, TemplateModel template)
        {
            string fullText = ExtractFullTextFromPdf(pdfPath);
            if (string.IsNullOrEmpty(fullText)) return new List<InvoiceModel>();

            var invoices = new List<InvoiceModel>();

            // Split full text into sections that represent different invoices
            var invoiceSections = SplitIntoInvoices(fullText, template);

            // Loop through each section and create an invoice model
            foreach (var section in invoiceSections)
            {
                var invoice = new InvoiceModel();
                foreach (var field in template.Fields.Values)
                {
                    string value = ExtractFieldValue(section, field.Keyword);
                    if (!string.IsNullOrEmpty(value))
                    {
                        AssignValueToInvoice(invoice, field.FieldName, value);
                    }
                }

                // Add invoice to the list only if it has meaningful data
                if (!string.IsNullOrEmpty(invoice.InvoiceNumber))
                {
                    invoices.Add(invoice);
                }
            }

            return invoices;
        }

        // Check if template matches any of the invoices in the PDF content using PdfPig
        public bool IsTemplateMatch(string pdfPath, TemplateModel template)
        {
            string fullText = ExtractFullTextFromPdf(pdfPath);
            if (string.IsNullOrEmpty(fullText)) return false;

            return template.Fields.Values.All(field => fullText.Contains(field.Keyword, StringComparison.OrdinalIgnoreCase));
        }

        // Convert PDF page to image using PDFtoImage
        public string ConvertPdfPageToImage(string pdfPath, int pageNumber = 1, int dpi = 300)
        {
            string outputImagePath = Path.Combine(Path.GetDirectoryName(pdfPath), $"pdf_page_{pageNumber}.png");

            try
            {
                using (var pdfStream = File.OpenRead(pdfPath))
                {
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

                    // Use advanced text extraction method to keep word separation intact
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

        // Split the full text into sections representing different invoices
        private List<string> SplitIntoInvoices(string fullText, TemplateModel template)
        {
            // Get all possible keywords from the template fields
            var possibleDelimiters = template.Fields.Values
                                              .Where(field => !string.IsNullOrWhiteSpace(field.Keyword))
                                              .Select(field => Regex.Escape(field.Keyword))
                                              .ToList();

            if (possibleDelimiters.Count == 0)
            {
                throw new InvalidOperationException("No keywords found in the template to split invoices.");
            }

            // Extract the first page's text (assuming '\f' is used to separate pages)
            string firstPageText = fullText.Split(new[] { '\f' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? fullText;

            // Build a regex pattern that matches any of the key fields in the first page
            string combinedPattern = string.Join("|", possibleDelimiters);

            // Find the first occurrence of any of the key fields in the first page
            var firstMatch = Regex.Match(firstPageText, combinedPattern);

            if (!firstMatch.Success)
            {
                // No keyword found in the first page, return the full document as a single section
                return new List<string> { fullText };
            }

            // Use the first field that appears as the delimiter
            string delimiter = firstMatch.Value;

            // Now, use this delimiter to split the full document into different invoices
            var invoiceSections = Regex.Split(fullText, Regex.Escape(delimiter))
                                       .Where(section => !string.IsNullOrWhiteSpace(section))
                                       .Select(section => delimiter + section.Trim()) // Re-add the delimiter at the start
                                       .ToList();

            return invoiceSections;
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
