using System.Windows;

namespace InvoiceExtractor.Services
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string message, string caption, MessageBoxButton buttons, MessageBoxImage icon);
    }
}
