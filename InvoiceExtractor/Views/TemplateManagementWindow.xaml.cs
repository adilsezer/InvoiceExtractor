using InvoiceExtractor.Services;
using InvoiceExtractor.ViewModels;
using System.Windows;

namespace InvoiceExtractor.Views
{
    public partial class TemplateManagementWindow : Window
    {
        public TemplateManagementWindow(IStorageService storageService)
        {
            InitializeComponent();
            DataContext = new TemplateViewModel(storageService);
        }
    }
}
