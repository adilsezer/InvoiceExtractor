using InvoiceExtractor.Models;
using InvoiceExtractor.Services;
using InvoiceExtractor.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace InvoiceExtractor.Views
{
    public partial class TemplateManagementWindow : Window
    {
        public TemplateManagementWindow(IStorageService storageService, IPdfProcessingService pdfProcessingService, IMessageBoxService messageBoxService, ObservableCollection<TemplateModel> templates)
        {
            InitializeComponent();
            DataContext = new TemplateViewModel(storageService, pdfProcessingService, messageBoxService, templates);
        }

        private void PdfImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(pdfImage);

            double xCoordinate = Math.Round(mousePosition.X, 2);
            double yCoordinate = Math.Round(mousePosition.Y, 2);

            if (DataContext is TemplateViewModel viewModel && viewModel.SelectedField != null)
            {
                viewModel.SelectedField.XCoordinate = xCoordinate;
                viewModel.SelectedField.YCoordinate = yCoordinate;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is TemplateViewModel viewModel)
            {
                if (viewModel.IsDirty)
                {
                    viewModel.SaveTemplateCommand.Execute(null);
                }
            }
        }
    }
}
