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

            // Initialize services
            IStorageService storageService = new JsonStorageService();
            IPdfProcessingService pdfService = new PdfProcessingService();

            // Set DataContext
            DataContext = new MainViewModel(pdfService, storageService);
        }
    }
}
