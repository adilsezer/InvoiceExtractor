namespace InvoiceExtractor.Models
{
    public class TemplateModel
    {
        public string TemplateName { get; set; } = string.Empty;
        public Dictionary<string, ExtractionField> Fields { get; set; } = new Dictionary<string, ExtractionField>();
    }
}
