using InvoiceExtractor.Models;
using InvoiceExtractor.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace InvoiceExtractor.Views
{
    public partial class TemplateSelectionWindow : Window
    {
        public TemplateModel? SelectedTemplate { get; private set; }

        public TemplateSelectionWindow(ObservableCollection<TemplateModel> templates)
        {
            InitializeComponent();
            DataContext = new TemplateSelectionViewModel(templates);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as TemplateSelectionViewModel;
            SelectedTemplate = viewModel?.SelectedTemplate;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
