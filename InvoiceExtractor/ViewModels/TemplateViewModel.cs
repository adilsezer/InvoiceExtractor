using InvoiceExtractor.Models;
using InvoiceExtractor.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
namespace InvoiceExtractor.ViewModels
{
    public class TemplateViewModel : BaseViewModel
    {
        private readonly IStorageService _storageService;

        public ObservableCollection<TemplateModel> Templates { get; set; }
        public ObservableCollection<ExtractionField> EditTemplateFields { get; set; }

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
                }
            }
        }

        public ICommand AddTemplateCommand { get; }
        public ICommand SaveTemplateCommand { get; }
        public ICommand DeleteTemplateCommand { get; }
        public ICommand AddFieldCommand { get; }
        public ICommand RemoveFieldCommand { get; }

        public TemplateViewModel(IStorageService storageService)
        {
            _storageService = storageService;
            Templates = new ObservableCollection<TemplateModel>(_storageService.LoadTemplates());
            EditTemplateFields = new ObservableCollection<ExtractionField>();

            AddTemplateCommand = new RelayCommand(AddTemplate);
            SaveTemplateCommand = new RelayCommand(SaveTemplate, CanSaveTemplate);
            AddFieldCommand = new RelayCommand(AddField);
            DeleteTemplateCommand = new RelayCommand(param => DeleteTemplate((TemplateModel)param), param => CanDeleteTemplate((TemplateModel)param));
            RemoveFieldCommand = new RelayCommand(param => RemoveField((ExtractionField)param), param => CanRemoveField((ExtractionField)param));

            // Trigger RaiseCanExecuteChanged when the fields collection changes
            EditTemplateFields.CollectionChanged += (s, e) =>
            {
                ((RelayCommand)SaveTemplateCommand).RaiseCanExecuteChanged();
            };
        }

        private void AddTemplate()
        {
            var newTemplate = new TemplateModel
            {
                TemplateName = "New Template",
                Fields = new Dictionary<string, ExtractionField>()
            };

            Templates.Add(newTemplate);
            SelectedTemplate = newTemplate;
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

        private void AddField()
        {
            if (SelectedTemplate != null)
            {
                var newField = new ExtractionField
                {
                    FieldName = "NewField",
                    Keyword = "Keyword",
                    XCoordinate = 0,
                    YCoordinate = 0
                };
                EditTemplateFields.Add(newField);

                // Notify that SaveTemplateCommand's execution state may have changed
                ((RelayCommand)SaveTemplateCommand).RaiseCanExecuteChanged();
            }
        }

        private bool CanRemoveField(ExtractionField field)
        {
            return field != null;
        }

        private void RemoveField(ExtractionField field)
        {
            if (field != null)
            {
                EditTemplateFields.Remove(field);

                // Notify that SaveTemplateCommand's execution state may have changed
                ((RelayCommand)SaveTemplateCommand).RaiseCanExecuteChanged();
            }
        }
    }
}