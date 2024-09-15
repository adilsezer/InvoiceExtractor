using CsvHelper;
using InvoiceExtractor.Models;
using System.Globalization;
using System.IO;

namespace InvoiceExtractor.Helpers
{
    public static class CsvExporter
    {
        public static void Export(IEnumerable<InvoiceModel> invoices, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteHeader<InvoiceModel>();
                csv.NextRecord();
                csv.WriteRecords(invoices);
            }
        }
    }
}
