using InvoiceExtractor.Models;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text.RegularExpressions;

namespace InvoiceExtractor.Services
{
    public class PdfProcessingService : IPdfProcessingService
    {
        public InvoiceModel ExtractInvoice(string pdfPath, TemplateModel template)
        {
            InvoiceModel invoice = new InvoiceModel();

            try
            {
                using (PdfReader reader = new PdfReader(pdfPath))
                using (PdfDocument pdfDoc = new PdfDocument(reader))
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    string fullText = "";

                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        var page = pdfDoc.GetPage(i);
                        string pageContent = PdfTextExtractor.GetTextFromPage(page, strategy);
                        fullText += pageContent + "\n";
                    }

                    foreach (var field in template.Fields.Values)
                    {
                        string pattern = $"{Regex.Escape(field.Keyword)}\\s*(.*)";
                        var match = Regex.Match(fullText, pattern, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            string value = match.Groups[1].Value.Trim();
                            AssignValueToInvoice(invoice, field.FieldName, value);
                        }
                    }
                }
            }
            catch
            {
                // Handle exceptions as needed
                return null!;
            }

            return invoice;
        }

        public bool IsTemplateMatch(string pdfPath, TemplateModel template)
        {
            try
            {
                using (PdfReader reader = new PdfReader(pdfPath))
                using (PdfDocument pdfDoc = new PdfDocument(reader))
                {
                    var strategy = new SimpleTextExtractionStrategy();
                    string fullText = "";

                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        var page = pdfDoc.GetPage(i);
                        string pageContent = PdfTextExtractor.GetTextFromPage(page, strategy);
                        fullText += pageContent + "\n";
                    }

                    // Check for the presence of all keywords in the template
                    foreach (var field in template.Fields.Values)
                    {
                        if (!fullText.Contains(field.Keyword, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch
            {
                // Handle exceptions as needed
                return false;
            }
        }

        private void AssignValueToInvoice(InvoiceModel invoice, string fieldName, string value)
        {
            switch (fieldName)
            {
                case "InvoiceNumber":
                    invoice.InvoiceNumber = value;
                    break;
                case "InvoiceDate":
                    if (DateTime.TryParse(value, out DateTime date))
                        invoice.InvoiceDate = date;
                    break;
                case "SellerDetails":
                    invoice.SellerDetails = value;
                    break;
                case "BuyerDetails":
                    invoice.BuyerDetails = value;
                    break;
                case "Description":
                    invoice.Description = value;
                    break;
                case "Amount":
                    if (decimal.TryParse(value, out decimal amount))
                        invoice.Amount = amount;
                    break;
                    // Add other fields as necessary
            }
        }
    }
}
