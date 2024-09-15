using InvoiceExtractor.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InvoiceExtractor.Views
{
    /// <summary>
    /// Interaction logic for TemplateManagementView.xaml
    /// </summary>
    public partial class TemplateManagementView : UserControl
    {
        public TemplateManagementView()
        {
            InitializeComponent();
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
    }
}
