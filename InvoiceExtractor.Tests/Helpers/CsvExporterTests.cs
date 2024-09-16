using InvoiceExtractor.Helpers;
using InvoiceExtractor.Models;

namespace InvoiceExtractor.Tests.Helpers
{
    public class CsvExporterTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _testFilePath;

        public CsvExporterTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_testDirectory);
            _testFilePath = Path.Combine(_testDirectory, "invoices_test.csv");
        }

        [Fact]
        public void CsvExporter_Export_CreatesCsvFileWithHeaderAndRecords()
        {
            // Arrange
            var invoices = new List<InvoiceModel>
            {
                new InvoiceModel
                {
                    InvoiceNumber = "INV-001",
                    InvoiceDate = "2023-10-01",
                    Vendor = "Vendor A",
                    Description = "Office Supplies",
                    Amount = 150.75m
                },
                new InvoiceModel
                {
                    InvoiceNumber = "INV-002",
                    InvoiceDate = "2023-10-05",
                    Vendor = "Vendor B",
                    Description = "Software Subscription",
                    Amount = 299.99m
                }
            };

            // Act
            CsvExporter.Export(invoices, _testFilePath);

            // Assert
            Assert.True(File.Exists(_testFilePath));

            var lines = File.ReadAllLines(_testFilePath);
            Assert.Equal(3, lines.Length); // Header + 2 records

            Assert.Equal("InvoiceNumber,InvoiceDate,Vendor,Description,Amount", lines[0]);
            Assert.Equal("INV-001,2023-10-01,Vendor A,Office Supplies,150.75", lines[1]);
            Assert.Equal("INV-002,2023-10-05,Vendor B,Software Subscription,299.99", lines[2]);
        }

        [Fact]
        public void CsvExporter_Export_CreatesOnlyHeader_WhenNoInvoices()
        {
            // Arrange
            var invoices = new List<InvoiceModel>();

            // Act
            CsvExporter.Export(invoices, _testFilePath);

            // Assert
            Assert.True(File.Exists(_testFilePath));

            var lines = File.ReadAllLines(_testFilePath);
            Assert.Single(lines); // Only header

            Assert.Equal("InvoiceNumber,InvoiceDate,Vendor,Description,Amount", lines[0]);
        }

        [Fact]
        public void CsvExporter_Export_ThrowsException_WhenFilePathIsInvalid()
        {
            // Arrange
            var invoices = new List<InvoiceModel>
            {
                new InvoiceModel { InvoiceNumber = "INV-001" }
            };
            var invalidFilePath = Path.Combine(_testDirectory, "invalid", "invoices.csv"); // 'invalid' directory does not exist

            // Act & Assert
            var exception = Assert.Throws<DirectoryNotFoundException>(() => CsvExporter.Export(invoices, invalidFilePath));
            Assert.Contains("Could not find a part of the path", exception.Message);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
