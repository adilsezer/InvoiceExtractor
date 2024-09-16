using InvoiceExtractor.Models;
using InvoiceExtractor.Services;
using System.Reflection;

namespace InvoiceExtractor.Tests.Services
{
    public class JsonStorageServiceTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _testFilePath;
        private readonly JsonStorageServiceMock _storageServiceMock;

        public JsonStorageServiceTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_testDirectory);
            _testFilePath = Path.Combine(_testDirectory, "templates.json");

            _storageServiceMock = new JsonStorageServiceMock(_testFilePath);
            _storageServiceMock.EnsureFileExists();
        }

        [Fact]
        public void JsonStorageService_LoadTemplates_ReturnsEmptyList_WhenFileIsEmpty()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "[]");

            // Act
            var templates = _storageServiceMock.LoadTemplates();

            // Assert
            Assert.NotNull(templates);
            Assert.Empty(templates);
        }

        [Fact]
        public void JsonStorageService_LoadTemplates_ReturnsLoadedTemplates()
        {
            // Arrange
            var templatesToSave = new List<TemplateModel>
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
            var json = System.Text.Json.JsonSerializer.Serialize(templatesToSave, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_testFilePath, json);

            // Act
            var loadedTemplates = _storageServiceMock.LoadTemplates();

            // Assert
            Assert.Single(loadedTemplates);
            var firstTemplate = loadedTemplates.First();
            Assert.Equal("Template A", firstTemplate.TemplateName);
            Assert.Equal(2, firstTemplate.Fields.Count);
            Assert.Contains("InvoiceNumber", firstTemplate.Fields.Keys);
            Assert.Contains("Amount", firstTemplate.Fields.Keys);
            Assert.Equal("INV#", firstTemplate.Fields["InvoiceNumber"].Keyword);
            Assert.Equal(100, firstTemplate.Fields["InvoiceNumber"].XCoordinate);
            Assert.Equal(200, firstTemplate.Fields["InvoiceNumber"].YCoordinate);
        }

        [Fact]
        public void JsonStorageService_SaveTemplates_CreatesValidJsonFile()
        {
            // Arrange
            var templatesToSave = new List<TemplateModel>
            {
                new TemplateModel
                {
                    TemplateName = "Template B",
                    Fields = new Dictionary<string, ExtractionField>
                    {
                        { "Vendor", new ExtractionField { FieldName = "Vendor", Keyword = "Vendor Name", XCoordinate = 150, YCoordinate = 250 } }
                    }
                }
            };

            // Act
            _storageServiceMock.SaveTemplates(templatesToSave);

            // Assert
            Assert.True(File.Exists(_testFilePath));

            var json = File.ReadAllText(_testFilePath);
            var loadedTemplates = System.Text.Json.JsonSerializer.Deserialize<List<TemplateModel>>(json);
            Assert.Single(loadedTemplates);
            Assert.Equal("Template B", loadedTemplates[0].TemplateName);
            Assert.Single(loadedTemplates[0].Fields);
            Assert.Contains("Vendor", loadedTemplates[0].Fields.Keys);
            Assert.Equal("Vendor Name", loadedTemplates[0].Fields["Vendor"].Keyword);
            Assert.Equal(150, loadedTemplates[0].Fields["Vendor"].XCoordinate);
            Assert.Equal(250, loadedTemplates[0].Fields["Vendor"].YCoordinate);
        }

        [Fact]
        public void JsonStorageService_SaveTemplates_OverwritesExistingFile()
        {
            // Arrange
            var initialTemplates = new List<TemplateModel>
            {
                new TemplateModel { TemplateName = "Initial Template" }
            };
            _storageServiceMock.SaveTemplates(initialTemplates);

            var updatedTemplates = new List<TemplateModel>
            {
                new TemplateModel { TemplateName = "Updated Template" }
            };

            // Act
            _storageServiceMock.SaveTemplates(updatedTemplates);
            var loadedTemplates = _storageServiceMock.LoadTemplates();

            // Assert
            Assert.Single(loadedTemplates);
            Assert.Equal("Updated Template", loadedTemplates.First().TemplateName);
        }

        [Fact]
        public void JsonStorageService_LoadTemplates_HandlesInvalidJson()
        {
            // Arrange
            File.WriteAllText(_testFilePath, "Invalid JSON");

            // Act & Assert
            Assert.Throws<System.Text.Json.JsonException>(() => _storageServiceMock.LoadTemplates());
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }

    public class JsonStorageServiceMock : JsonStorageService
    {
        public JsonStorageServiceMock(string filePath)
        {
            SetFilePath(filePath);
        }

        public void SetFilePath(string filePath)
        {
            var field = typeof(JsonStorageService).GetField("_filePath", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                throw new InvalidOperationException("Unable to find _filePath field in JsonStorageService");

            field.SetValue(this, filePath);
            EnsureFileExists();
        }
    }
}
