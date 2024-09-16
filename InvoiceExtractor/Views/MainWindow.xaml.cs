using InvoiceExtractor.Services;
using InvoiceExtractor.ViewModels;
using System.Windows;

namespace InvoiceExtractor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            IStorageService storageService = new JsonStorageService();
            IPdfProcessingService pdfService = new PdfProcessingService();
            IMessageBoxService messageBoxService = new MessageBoxService();

            DataContext = new MainViewModel(pdfService, storageService, messageBoxService);
        }
    }
}
