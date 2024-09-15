namespace InvoiceExtractor.Models
{
    public class InvoiceModel
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string SellerDetails { get; set; } = string.Empty;
        public string BuyerDetails { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
