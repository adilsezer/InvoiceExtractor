using System.ComponentModel;

namespace InvoiceExtractor.Models
{
    public class ExtractionField : INotifyPropertyChanged
    {
        private string _fieldName = string.Empty;
        private string _keyword = string.Empty;
        private double _xCoordinate;
        private double _yCoordinate;

        public string FieldName
        {
            get => _fieldName;
            set
            {
                _fieldName = value;
                OnPropertyChanged(nameof(FieldName));
            }
        }

        public string Keyword
        {
            get => _keyword;
            set
            {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
            }
        }

        public double XCoordinate
        {
            get => _xCoordinate;
            set
            {
                _xCoordinate = value;
                OnPropertyChanged(nameof(XCoordinate));
            }
        }

        public double YCoordinate
        {
            get => _yCoordinate;
            set
            {
                _yCoordinate = value;
                OnPropertyChanged(nameof(YCoordinate));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
