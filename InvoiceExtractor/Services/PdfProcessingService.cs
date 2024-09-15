using InvoiceExtractor.Models;
using PDFtoImage;
using System.IO;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace InvoiceExtractor.Services
{
    public class PdfProcessingService : IPdfProcessingService
    {
        // Extracts multiple invoice details from a single PDF, treating each page as a new invoice
        public List<InvoiceModel> ExtractInvoices(string pdfPath, TemplateModel template)
        {
            var invoices = new List<InvoiceModel>();

            try
            {
                using (var document = PdfDocument.Open(pdfPath))
                {
                    // Loop through each page of the document
                    foreach (var page in document.GetPages())
                    {
                        // Extract text for the current page
                        string pageText = ContentOrderTextExtractor.GetText(page);

                        // Create a new invoice model for each page
                        var invoice = new InvoiceModel();

                        // Extract fields based on the template for each page
                        foreach (var field in template.Fields.Values)
                        {
                            string value = ExtractFieldValue(pageText, field.Keyword);

                            // If regex extraction fails, attempt to use coordinates for extraction
                            if (string.IsNullOrEmpty(value) && field.XCoordinate > 0 && field.YCoordinate > 0)
                            {
                                value = ExtractTextUsingCoordinates(pdfPath, field, page.Number);
                            }

                            if (!string.IsNullOrEmpty(value))
                            {
                                AssignValueToInvoice(invoice, field.FieldName, value);
                            }
                        }

                        // Add the invoice if it has meaningful data (e.g., InvoiceNumber is populated)
                        if (!string.IsNullOrEmpty(invoice.InvoiceNumber))
                        {
                            invoices.Add(invoice);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting invoices: {ex.Message}");
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

                    // Extract text from each page
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

        // Extract text using coordinates if regex fails
        private string ExtractTextUsingCoordinates(string pdfPath, ExtractionField field, int pageNumber)
        {
            try
            {
                using (var document = PdfDocument.Open(pdfPath))
                {
                    var page = document.GetPage(pageNumber);

                    // Define a bounding box around the coordinates
                    var boundingBox = new PdfRectangle(
                        field.XCoordinate, field.YCoordinate,
                        field.XCoordinate + 50, field.YCoordinate + 20);

                    // Get all letters on the page
                    var letters = page.Letters;

                    // Filter letters that fall within the bounding box
                    var lettersInBox = letters.Where(letter =>
                        letter.GlyphRectangle.Left >= boundingBox.Left &&
                        letter.GlyphRectangle.Right <= boundingBox.Right &&
                        letter.GlyphRectangle.Bottom >= boundingBox.Bottom &&
                        letter.GlyphRectangle.Top <= boundingBox.Top);

                    // Combine the filtered letters into a string
                    var extractedText = string.Concat(lettersInBox.Select(letter => letter.Value));

                    if (!string.IsNullOrEmpty(extractedText))
                    {
                        return extractedText.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting text from PDF using coordinates: {ex.Message}");
            }

            return string.Empty;
        }
    }
}
