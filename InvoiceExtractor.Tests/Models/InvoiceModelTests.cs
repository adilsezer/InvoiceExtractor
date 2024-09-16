using InvoiceExtractor.Models;

namespace InvoiceExtractor.Tests.Models
{
    public class InvoiceModelTests
    {
        [Fact]
        public void InvoiceModel_DefaultValues_AreSetCorrectly()
        {
            // Arrange
            var invoice = new InvoiceModel();

            // Act & Assert
            Assert.Equal(string.Empty, invoice.InvoiceNumber);
            Assert.Null(invoice.InvoiceDate);
            Assert.Equal(string.Empty, invoice.Vendor);
            Assert.Equal(string.Empty, invoice.Description);
            Assert.Equal(0m, invoice.Amount);
        }

        [Fact]
        public void InvoiceModel_SetProperties_WorksCorrectly()
        {
            // Arrange
            var invoice = new InvoiceModel
            {
                InvoiceNumber = "INV-123",
                InvoiceDate = "2023-10-15",
                Vendor = "Vendor A",
                Description = "Office Supplies",
                Amount = 250.75m
            };

            // Act & Assert
            Assert.Equal("INV-123", invoice.InvoiceNumber);
            Assert.Equal("2023-10-15", invoice.InvoiceDate);
            Assert.Equal("Vendor A", invoice.Vendor);
            Assert.Equal("Office Supplies", invoice.Description);
            Assert.Equal(250.75m, invoice.Amount);
        }
    }
}
