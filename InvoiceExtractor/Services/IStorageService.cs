using InvoiceExtractor.Models;

namespace InvoiceExtractor.Services
{
    public interface IStorageService
    {
        IEnumerable<TemplateModel> LoadTemplates();
        void SaveTemplates(IEnumerable<TemplateModel> templates);
    }
}
