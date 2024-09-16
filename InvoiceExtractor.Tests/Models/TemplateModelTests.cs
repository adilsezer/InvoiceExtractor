using InvoiceExtractor.Models;

namespace InvoiceExtractor.Tests.Models
{
    public class TemplateModelTests
    {
        [Fact]
        public void TemplateModel_DefaultValues_AreSetCorrectly()
        {
            // Arrange
            var template = new TemplateModel();

            // Act & Assert
            Assert.Equal(string.Empty, template.TemplateName);
            Assert.NotNull(template.Fields);
            Assert.Empty(template.Fields);
        }

        [Fact]
        public void TemplateModel_SetProperties_WorksCorrectly()
        {
            // Arrange
            var fields = new Dictionary<string, ExtractionField>
            {
                { "InvoiceNumber", new ExtractionField { FieldName = "InvoiceNumber", Keyword = "INV#", XCoordinate = 100, YCoordinate = 200 } },
                { "Amount", new ExtractionField { FieldName = "Amount", Keyword = "Total", XCoordinate = 300, YCoordinate = 400 } }
            };

            var template = new TemplateModel
            {
                TemplateName = "Template A",
                Fields = fields
            };

            // Act & Assert
            Assert.Equal("Template A", template.TemplateName);
            Assert.Equal(2, template.Fields.Count);
            Assert.Contains("InvoiceNumber", template.Fields.Keys);
            Assert.Contains("Amount", template.Fields.Keys);
        }
    }
}
