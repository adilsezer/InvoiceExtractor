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
        public TemplateManagementWindow(IStorageService storageService, IPdfProcessingService pdfProcessingService, ObservableCollection<TemplateModel> templates)
        {
            InitializeComponent();
            DataContext = new TemplateViewModel(storageService, pdfProcessingService, templates);
        }

        private void PdfImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get mouse position relative to the image
            Point mousePosition = e.GetPosition(pdfImage);

            // Calculate X, Y coordinates (scaled appropriately for the image)
            double xCoordinate = Math.Round(mousePosition.X, 2);
            double yCoordinate = Math.Round(mousePosition.Y, 2);

            // Check if a field is selected in the ViewModel
            if (DataContext is TemplateViewModel viewModel && viewModel.SelectedField != null)
            {
                // Set the coordinates on the selected field
                viewModel.SelectedField.XCoordinate = xCoordinate;
                viewModel.SelectedField.YCoordinate = yCoordinate;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is TemplateViewModel viewModel)
            {
                // Check if there are unsaved changes
                if (viewModel.IsDirty)
                {
                    viewModel.SaveTemplateCommand.Execute(null);
                }
            }
        }
    }
}
