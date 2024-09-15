using InvoiceExtractor.Models;

namespace InvoiceExtractor.Services
{
    public interface IPdfProcessingService
    {
        InvoiceModel ExtractInvoice(string pdfPath, TemplateModel template);
        bool IsTemplateMatch(string pdfPath, TemplateModel template);
    }
}
