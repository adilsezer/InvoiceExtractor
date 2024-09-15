using InvoiceExtractor.Models;
using System.Collections.ObjectModel;

namespace InvoiceExtractor.ViewModels
{
    public class TemplateSelectionViewModel : BaseViewModel
    {
        public ObservableCollection<TemplateModel> Templates { get; set; }

        private TemplateModel _selectedTemplate;
        public TemplateModel SelectedTemplate
        {
            get => _selectedTemplate;
            set => SetProperty(ref _selectedTemplate, value);
        }

        public TemplateSelectionViewModel(ObservableCollection<TemplateModel> templates)
        {
            Templates = templates;
            if (templates.Count > 0)
            {
                SelectedTemplate = templates[0];
            }
        }
    }
}