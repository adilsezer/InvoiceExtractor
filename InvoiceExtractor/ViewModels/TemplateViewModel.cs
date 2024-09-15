﻿using InvoiceExtractor.Models;
using InvoiceExtractor.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace InvoiceExtractor.ViewModels
{
    public class TemplateViewModel : BaseViewModel
    {
        private readonly IStorageService _storageService;
        private readonly IPdfProcessingService _pdfProcessingService;

        public ObservableCollection<TemplateModel> Templates { get; set; }
        public ObservableCollection<ExtractionField> EditTemplateFields { get; set; }

        private BitmapImage _pdfImageSource;
        public BitmapImage PdfImageSource
        {
            get => _pdfImageSource;
            set
            {
                _pdfImageSource = value;
                OnPropertyChanged(nameof(PdfImageSource));
            }
        }

        private TemplateModel _selectedTemplate;
        public TemplateModel SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                if (SetProperty(ref _selectedTemplate, value))
                {
                    EditTemplateFields.Clear();

                    if (_selectedTemplate != null)
                    {
                        foreach (var field in _selectedTemplate.Fields.Values)
                        {
                            EditTemplateFields.Add(field);
                        }
                    }

                    // Notify the Save command to re-evaluate if it can execute
                    ((RelayCommand)SaveTemplateCommand).RaiseCanExecuteChanged();
                    OnPropertyChanged(nameof(IsTemplateSelected));
                }
            }
        }

        private ExtractionField _selectedField;
        public ExtractionField SelectedField
        {
            get => _selectedField;
            set
            {
                if (SetProperty(ref _selectedField, value))
                {
                    // Notify that the CanRemoveField command needs to be re-evaluated
                    OnPropertyChanged(nameof(SelectedField));
                }
            }
        }

        private bool _isDirty = false; // Tracks whether there are unsaved changes

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (SetProperty(ref _isDirty, value))
                {
                    // Notify SaveTemplateCommand that it can execute or not
                    ((RelayCommand)SaveTemplateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsTemplateSelected => SelectedTemplate != null;

        public ICommand AddTemplateCommand { get; }
        public ICommand SaveTemplateCommand { get; }
        public ICommand DeleteTemplateCommand { get; }
        public ICommand LoadPdfCommand { get; }

        public TemplateViewModel(IStorageService storageService, IPdfProcessingService pdfProcessingService, ObservableCollection<TemplateModel> templates)
        {
            _storageService = storageService;
            _pdfProcessingService = pdfProcessingService;
            Templates = templates;
            EditTemplateFields = new ObservableCollection<ExtractionField>();

            AddTemplateCommand = new RelayCommand(AddTemplate);
            SaveTemplateCommand = new RelayCommand(SaveTemplate, CanSaveTemplate);
            DeleteTemplateCommand = new RelayCommand(param => DeleteTemplate((TemplateModel)param), param => CanDeleteTemplate((TemplateModel)param));
            LoadPdfCommand = new RelayCommand(LoadPdf);

            // Trigger RaiseCanExecuteChanged when the fields collection changes
            EditTemplateFields.CollectionChanged += (s, e) =>
            {
                MarkAsDirty();
                ((RelayCommand)SaveTemplateCommand).RaiseCanExecuteChanged();
            };
        }

        private void AddTemplate()
        {
            // Find a unique template name
            string baseName = "New Template";
            int counter = 1;

            // Keep incrementing the counter until a unique name is found
            string newTemplateName = baseName;
            while (Templates.Any(t => t.TemplateName == newTemplateName))
            {
                newTemplateName = $"{baseName} {counter++}";
            }

            // Create a new template with the unique name and predefined fields
            var newTemplate = new TemplateModel
            {
                TemplateName = newTemplateName,
                Fields = new Dictionary<string, ExtractionField>
        {
            { "InvoiceNumber", new ExtractionField { FieldName = "InvoiceNumber", Keyword = "Invoice Number", XCoordinate = 0, YCoordinate = 0 } },
            { "InvoiceDate", new ExtractionField { FieldName = "InvoiceDate", Keyword = "Invoice Date", XCoordinate = 0, YCoordinate = 0 } },
            { "Vendor", new ExtractionField { FieldName = "Vendor", Keyword = "Vendor", XCoordinate = 0, YCoordinate = 0 } },
            { "Description", new ExtractionField { FieldName = "Description", Keyword = "Description", XCoordinate = 0, YCoordinate = 0 } },
            { "Amount", new ExtractionField { FieldName = "Amount", Keyword = "Amount", XCoordinate = 0, YCoordinate = 0 } }
        }
            };

            Templates.Add(newTemplate);
            SelectedTemplate = newTemplate;
        }

        private void MarkAsDirty()
        {
            IsDirty = true;
        }

        private void LoadPdf()
        {
            // Open file dialog to select the PDF
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Convert the PDF to an image and set PdfImageSource
                string imagePath = _pdfProcessingService.ConvertPdfPageToImage(openFileDialog.FileName);
                PdfImageSource = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            }
        }

        private bool CanSaveTemplate()
        {
            return SelectedTemplate != null && !string.IsNullOrWhiteSpace(SelectedTemplate.TemplateName) && EditTemplateFields.Any();
        }

        private void SaveTemplate()
        {
            if (SelectedTemplate != null)
            {
                SelectedTemplate.Fields = EditTemplateFields.ToDictionary(f => f.FieldName, f => f);
                _storageService.SaveTemplates(Templates);
                IsDirty = false;
                MessageBox.Show("Template saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanDeleteTemplate(TemplateModel template)
        {
            return template != null;
        }

        private void DeleteTemplate(TemplateModel template)
        {
            if (template != null && MessageBox.Show($"Are you sure you want to delete the template '{template.TemplateName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Templates.Remove(template);
                _storageService.SaveTemplates(Templates);
            }
        }
    }
}