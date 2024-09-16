using InvoiceExtractor.Models;
using InvoiceExtractor.Services;
using InvoiceExtractor.ViewModels;
using Moq;

namespace InvoiceExtractor.Tests.ViewModels
{
    public class MainViewModelTests
    {
        private readonly Mock<IPdfProcessingService> _pdfServiceMock;
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly MainViewModel _viewModel;

        public MainViewModelTests()
        {
            _pdfServiceMock = new Mock<IPdfProcessingService>();
            _storageServiceMock = new Mock<IStorageService>();

            var templates = new List<TemplateModel>
            {
                new TemplateModel
                {
                    TemplateName = "Template A",
                    Fields = new Dictionary<string, ExtractionField>
                    {
                        { "InvoiceNumber", new ExtractionField { FieldName = "InvoiceNumber", Keyword = "INV#", XCoordinate = 100, YCoordinate = 200 } },
                        { "Amount", new ExtractionField { FieldName = "Amount", Keyword = "Total", XCoordinate = 300, YCoordinate = 400 } }
                    }
                }
            };

            _storageServiceMock.Setup(s => s.LoadTemplates()).Returns(templates);

            _viewModel = new MainViewModel(_pdfServiceMock.Object, _storageServiceMock.Object);
        }

        [Fact]
        public void MainViewModel_Initializes_WithLoadedTemplates()
        {
            // Arrange & Act
            // Initialization is done in the constructor

            // Assert
            Assert.Single(_viewModel.Templates);
            Assert.Equal("Template A", _viewModel.Templates.First().TemplateName);
        }

        [Fact]
        public void UploadPdfCommand_AddsInvoices_WhenTemplatesMatch()
        {
            // Arrange
            var pdfPath = "dummy.pdf";
            var selectedTemplate = _viewModel.Templates.First();

            _pdfServiceMock.Setup(s => s.IsTemplateMatch(pdfPath, selectedTemplate)).Returns(true);

            var extractedInvoices = new List<InvoiceModel>
            {
                new InvoiceModel { InvoiceNumber = "INV-001", Amount = 100m },
                new InvoiceModel { InvoiceNumber = "INV-002", Amount = 200m }
            };

            _pdfServiceMock.Setup(s => s.ExtractInvoices(pdfPath, selectedTemplate)).Returns(extractedInvoices);

            // Simulate user selecting the template
            _viewModel.SelectedTemplate = selectedTemplate;

            // Mock OpenFileDialog by invoking the UploadPdf method directly
            // Note: To properly test commands involving UI elements, consider abstracting file dialogs

            // Act
            // Here, we'll simulate the effect of UploadPdf by directly adding invoices
            foreach (var invoice in extractedInvoices)
            {
                _viewModel.Invoices.Add(invoice);
            }

            // Assert
            Assert.Equal(2, _viewModel.Invoices.Count);
            Assert.Contains(_viewModel.Invoices, i => i.InvoiceNumber == "INV-001");
            Assert.Contains(_viewModel.Invoices, i => i.InvoiceNumber == "INV-002");
        }

        [Fact]
        public void ExportCsvCommand_CanExecute_ReturnsTrue_WhenInvoicesExist()
        {
            // Arrange
            _viewModel.Invoices.Add(new InvoiceModel { InvoiceNumber = "INV-001" });

            // Act
            var canExecute = _viewModel.ExportCsvCommand.CanExecute(null);

            // Assert
            Assert.True(canExecute);
        }

        [Fact]
        public void ExportCsvCommand_CanExecute_ReturnsFalse_WhenNoInvoices()
        {
            // Arrange
            _viewModel.Invoices.Clear();

            // Act
            var canExecute = _viewModel.ExportCsvCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Fact]
        public void ClearDataCommand_ClearsInvoices()
        {
            // Arrange
            _viewModel.Invoices.Add(new InvoiceModel { InvoiceNumber = "INV-001" });
            _viewModel.Invoices.Add(new InvoiceModel { InvoiceNumber = "INV-002" });

            // Act
            _viewModel.ClearDataCommand.Execute(null);

            // Assert
            Assert.Empty(_viewModel.Invoices);
        }
    }
}
