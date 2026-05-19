using System.Globalization;
using CsvHelper;

namespace gasmie.src
{
    public static class CsvGenerator
    {
        public static void Generate(List<object> dtos)
        {
            if (dtos is null || dtos.Count == 0)
            {
                Console.WriteLine("Warning: No data to generate CSV.");
                return;
            }

            var filePath = CreatePath(dtos.First());
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(dtos);
            
            Console.WriteLine($"CSV generated: {filePath}");
        }

        private static string CreatePath(object dto)
        {
            var folderName = dto.ToString();
            if (string.IsNullOrEmpty(folderName))
                folderName = "exports";

            var path = Path.Combine("files", folderName);
            Directory.CreateDirectory(path);
            
            return Path.Combine(path, $"{DateTime.Now.ToFileTime()}.csv");
        }
    }
}