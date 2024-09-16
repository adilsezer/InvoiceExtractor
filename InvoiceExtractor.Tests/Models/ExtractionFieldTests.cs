using InvoiceExtractor.Models;

namespace InvoiceExtractor.Tests.Models
{
    public class ExtractionFieldTests
    {
        [Fact]
        public void ExtractionField_PropertyChanged_IsRaised_OnFieldNameChange()
        {
            // Arrange
            var field = new ExtractionField();
            string changedProperty = null;
            bool eventRaised = false;

            field.PropertyChanged += (sender, args) =>
            {
                changedProperty = args.PropertyName;
                eventRaised = true;
            };

            // Act
            field.FieldName = "NewFieldName";

            // Assert
            Assert.True(eventRaised);
            Assert.Equal(nameof(ExtractionField.FieldName), changedProperty);
        }

        [Fact]
        public void ExtractionField_PropertyChanged_IsRaised_OnKeywordChange()
        {
            // Arrange
            var field = new ExtractionField();
            string changedProperty = null;
            bool eventRaised = false;

            field.PropertyChanged += (sender, args) =>
            {
                changedProperty = args.PropertyName;
                eventRaised = true;
            };

            // Act
            field.Keyword = "NewKeyword";

            // Assert
            Assert.True(eventRaised);
            Assert.Equal(nameof(ExtractionField.Keyword), changedProperty);
        }

        [Fact]
        public void ExtractionField_PropertyChanged_IsRaised_OnXCoordinateChange()
        {
            // Arrange
            var field = new ExtractionField();
            string changedProperty = null;
            bool eventRaised = false;

            field.PropertyChanged += (sender, args) =>
            {
                changedProperty = args.PropertyName;
                eventRaised = true;
            };

            // Act
            field.XCoordinate = 150.75;

            // Assert
            Assert.True(eventRaised);
            Assert.Equal(nameof(ExtractionField.XCoordinate), changedProperty);
        }

        [Fact]
        public void ExtractionField_PropertyChanged_IsRaised_OnYCoordinateChange()
        {
            // Arrange
            var field = new ExtractionField();
            string changedProperty = null;
            bool eventRaised = false;

            field.PropertyChanged += (sender, args) =>
            {
                changedProperty = args.PropertyName;
                eventRaised = true;
            };

            // Act
            field.YCoordinate = 250.50;

            // Assert
            Assert.True(eventRaised);
            Assert.Equal(nameof(ExtractionField.YCoordinate), changedProperty);
        }

        [Fact]
        public void ExtractionField_PropertyChanged_NotRaised_WhenValueUnchanged()
        {
            // Arrange
            var field = new ExtractionField { FieldName = "InitialName" };
            bool eventRaised = false;

            field.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
            };

            // Act
            field.FieldName = "InitialName"; // Setting the same value

            // Assert
            Assert.False(eventRaised);
        }
    }
}
