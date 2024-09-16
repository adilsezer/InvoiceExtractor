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
            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<TemplateModel>>(json) ?? new List<TemplateModel>();
            }
            catch (JsonException ex)
            {
                // Handle or log the error appropriately
                Console.WriteLine($"Invalid JSON format: {ex.Message}");
                throw; // Re-throwing the exception to ensure the test catches it
            }
        }

        public void SaveTemplates(IEnumerable<TemplateModel> templates)
        {
            var json = JsonSerializer.Serialize(templates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void EnsureFileExists()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }
    }
}
