using InvoiceExtractor.Models;

namespace InvoiceExtractor.Services
{
    public interface IPdfProcessingService
    {
        List<InvoiceModel> ExtractInvoices(string pdfPath, TemplateModel template);
        bool IsTemplateMatch(string pdfPath, TemplateModel template);
        string? ConvertPdfPageToImage(string pdfPath, int pageNumber = 1, int dpi = 300);
    }
}
