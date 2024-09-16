using InvoiceExtractor.Models;
using InvoiceExtractor.Services;
using InvoiceExtractor.ViewModels;
using Moq;
using System.Collections.ObjectModel;

namespace InvoiceExtractor.Tests.ViewModels
{
    public class TemplateViewModelTests
    {
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly Mock<IPdfProcessingService> _pdfProcessingServiceMock;
        private readonly ObservableCollection<TemplateModel> _templates;
        private readonly TemplateViewModel _viewModel;

        public TemplateViewModelTests()
        {
            _storageServiceMock = new Mock<IStorageService>();
            _pdfProcessingServiceMock = new Mock<IPdfProcessingService>();

            _templates = new ObservableCollection<TemplateModel>
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

            _viewModel = new TemplateViewModel(_storageServiceMock.Object, _pdfProcessingServiceMock.Object, _templates);
        }

        [Fact]
        public void TemplateViewModel_Initializes_WithExistingTemplates()
        {
            // Arrange & Act
            // Initialization done in constructor

            // Assert
            Assert.Single(_viewModel.Templates);
            Assert.Equal("Template A", _viewModel.Templates[0].TemplateName);
        }

        [Fact]
        public void AddTemplateCommand_AddsNewTemplate_WithUniqueName()
        {
            // Arrange
            _viewModel.Templates.Add(new TemplateModel { TemplateName = "New Template" });

            // Act
            _viewModel.AddTemplateCommand.Execute(null);

            // Assert
            Assert.Equal(3, _viewModel.Templates.Count);
            Assert.Contains(_viewModel.Templates, t => t.TemplateName == "New Template 1");
        }

        [Fact]
        public void SaveTemplateCommand_SavesChanges_AndResetsIsDirty()
        {
            // Arrange
            var template = _viewModel.Templates.First();
            _viewModel.SelectedTemplate = template;
            _viewModel.EditTemplateFields.Add(new ExtractionField { FieldName = "Vendor", Keyword = "Vendor Name" });

            // Act
            _viewModel.SaveTemplateCommand.Execute(null);

            // Assert
            _storageServiceMock.Verify(s => s.SaveTemplates(It.IsAny<IEnumerable<TemplateModel>>()), Times.Once);
            Assert.False(_viewModel.IsDirty);
        }

        [Fact]
        public void DeleteTemplateCommand_RemovesSelectedTemplate()
        {
            // Arrange
            var templateToDelete = _viewModel.Templates.First();
            _viewModel.SelectedTemplate = templateToDelete;

            // Act
            _viewModel.DeleteTemplateCommand.Execute(templateToDelete);

            // Assert
            Assert.Empty(_viewModel.Templates);
            _storageServiceMock.Verify(s => s.SaveTemplates(It.IsAny<IEnumerable<TemplateModel>>()), Times.Once);
        }

        [Fact]
        public void TemplateViewModel_IsDirty_SetToTrue_WhenFieldsChange()
        {
            // Arrange
            var template = _viewModel.Templates.First();
            _viewModel.SelectedTemplate = template;

            // Act
            _viewModel.EditTemplateFields.Add(new ExtractionField { FieldName = "Vendor", Keyword = "Vendor Name" });

            // Assert
            Assert.True(_viewModel.IsDirty);
        }

        [Fact]
        public void SaveTemplateCommand_CanExecute_ReturnsFalse_WhenNoTemplateSelected()
        {
            // Arrange
            _viewModel.SelectedTemplate = null;

            // Act
            var canExecute = _viewModel.SaveTemplateCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Fact]
        public void SaveTemplateCommand_CanExecute_ReturnsFalse_WhenTemplateNameIsEmpty()
        {
            // Arrange
            var template = _viewModel.Templates.First();
            _viewModel.SelectedTemplate = template;
            _viewModel.SelectedTemplate.TemplateName = "";

            // Act
            var canExecute = _viewModel.SaveTemplateCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Fact]
        public void SaveTemplateCommand_CanExecute_ReturnsFalse_WhenNoFields()
        {
            // Arrange
            var template = _viewModel.Templates.First();
            _viewModel.SelectedTemplate = template;
            _viewModel.EditTemplateFields.Clear();

            // Act
            var canExecute = _viewModel.SaveTemplateCommand.CanExecute(null);

            // Assert
            Assert.False(canExecute);
        }

        [Fact]
        public void SaveTemplateCommand_CanExecute_ReturnsTrue_WhenValid()
        {
            // Arrange
            var template = _viewModel.Templates.First();
            _viewModel.SelectedTemplate = template;
            _viewModel.EditTemplateFields.Add(new ExtractionField { FieldName = "Vendor", Keyword = "Vendor Name" });

            // Act
            var canExecute = _viewModel.SaveTemplateCommand.CanExecute(null);

            // Assert
            Assert.True(canExecute);
        }
    }
}
