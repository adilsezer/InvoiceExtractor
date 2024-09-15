namespace InvoiceExtractor.Models
{
    public class ExtractionField
    {
        public string FieldName { get; set; } = string.Empty;
        public string Keyword { get; set; } = string.Empty;
        public double XCoordinate { get; set; }
        public double YCoordinate { get; set; }
    }
}
