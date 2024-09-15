using InvoiceExtractor.Helpers;
using InvoiceExtractor.Models;
using InvoiceExtractor.Services;
using InvoiceExtractor.Views;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace InvoiceExtractor.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IPdfProcessingService _pdfService;
        private readonly IStorageService _storageService;

        public ObservableCollection<InvoiceModel> Invoices { get; set; }
        public ObservableCollection<TemplateModel> Templates { get; set; }

        private TemplateModel _selectedTemplate;
        public TemplateModel SelectedTemplate
        {
            get => _selectedTemplate;
            set => SetProperty(ref _selectedTemplate, value);
        }

        private InvoiceModel _selectedInvoice;
        public InvoiceModel SelectedInvoice
        {
            get => _selectedInvoice;
            set => SetProperty(ref _selectedInvoice, value);
        }

        public ICommand UploadPdfCommand { get; }
        public ICommand ExportCsvCommand { get; }
        public ICommand ManageTemplatesCommand { get; }
        public ICommand ClearDataCommand { get; }

        public MainViewModel(IPdfProcessingService pdfService, IStorageService storageService)
        {
            _pdfService = pdfService;
            _storageService = storageService;

            Invoices = new ObservableCollection<InvoiceModel>();
            Templates = new ObservableCollection<TemplateModel>(_storageService.LoadTemplates());

            UploadPdfCommand = new RelayCommand(UploadPdf);
            ExportCsvCommand = new RelayCommand(ExportCsv, CanExportCsv);
            ManageTemplatesCommand = new RelayCommand(ManageTemplates);
            ClearDataCommand = new RelayCommand(ClearData);

            // Subscribe to collection changes to update command states
            Invoices.CollectionChanged += (s, e) => ((RelayCommand)ExportCsvCommand).RaiseCanExecuteChanged();
        }

        private void ClearData()
        {
            Invoices = new ObservableCollection<InvoiceModel>();
            OnPropertyChanged(nameof(Invoices));
        }

        private async void UploadPdf()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var filePath in openFileDialog.FileNames)
                {
                    var matchedTemplate = await Task.Run(() => DetectTemplate(filePath));

                    if (matchedTemplate != null)
                    {
                        var invoices = await Task.Run(() => _pdfService.ExtractInvoices(filePath, matchedTemplate));
                        if (invoices != null && invoices.Count > 0)
                        {
                            foreach (var invoice in invoices)
                            {
                                Application.Current.Dispatcher.Invoke(() => Invoices.Add(invoice));
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Failed to extract data from {Path.GetFileName(filePath)} using template {matchedTemplate.TemplateName}.", "Extraction Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"No matching template was found for the uploaded invoice.. Please verify the file and try again, or consider adding a new template to handle this type of document.",
                                        "Template Not Found",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                    }
                }
            }
        }

        private TemplateModel DetectTemplate(string pdfPath)
        {
            foreach (var template in Templates)
            {
                if (_pdfService.IsTemplateMatch(pdfPath, template))
                {
                    return template;
                }
            }
            return null;
        }

        private bool CanExportCsv()
        {
            return Invoices.Count > 0;
        }

        private async void ExportCsv()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv",
                FileName = "Invoices.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    await Task.Run(() => CsvExporter.Export(Invoices, saveFileDialog.FileName));
                    MessageBox.Show("CSV export successful.", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to export CSV: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ManageTemplates()
        {
            var templateManagementWindow = new TemplateManagementWindow(_storageService, _pdfService, Templates);

            if (templateManagementWindow.ShowDialog() == true)
            {
                // Reload updated templates from the storage service
                var updatedTemplates = _storageService.LoadTemplates();

                // Instead of clearing and repopulating, reset the ObservableCollection
                Templates = new ObservableCollection<TemplateModel>(updatedTemplates);

                // Notify the UI that the Templates property has changed
                OnPropertyChanged(nameof(Templates));

                // Optionally, set SelectedTemplate to the first one, if needed
                if (Templates.Count > 0)
                {
                    SelectedTemplate = Templates[0];
                }
            }
        }
    }
}