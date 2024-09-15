using InvoiceExtractor.Models;
using InvoiceExtractor.Services;
using InvoiceExtractor.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace InvoiceExtractor.Views
{
    public partial class TemplateManagementWindow : Window
    {
        public TemplateManagementWindow(IStorageService storageService, IPdfProcessingService pdfProcessingService, ObservableCollection<TemplateModel> templates)
        {
            InitializeComponent();
            DataContext = new TemplateViewModel(storageService, pdfProcessingService, templates);
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
