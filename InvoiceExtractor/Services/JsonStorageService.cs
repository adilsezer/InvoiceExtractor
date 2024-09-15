using InvoiceExtractor.Models;
using System.IO;
using System.Text.Json;

namespace InvoiceExtractor.Services
{
    public class JsonStorageService : IStorageService
    {
        private readonly string _filePath;

        public JsonStorageService()
        {
            _filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "InvoiceExtractor",
                "templates.json"
            );
            EnsureFileExists();
        }

        public IEnumerable<TemplateModel> LoadTemplates()
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TemplateModel>>(json) ?? new List<TemplateModel>();
        }

        public void SaveTemplates(IEnumerable<TemplateModel> templates)
        {
            var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        private void EnsureFileExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }
    }
}
