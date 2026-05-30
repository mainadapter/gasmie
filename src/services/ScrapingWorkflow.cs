using System.Collections.Concurrent;
using gasmie.src.models;
using gasmie.src.scrapers;

namespace gasmie.src.services
{
    public class ScrapingWorkflow
    {
        public static async Task RunAsync(List<ScraperDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
            {
                Console.WriteLine("No scrapers to run.");
                return;
            }

            Console.WriteLine($"Starting concurrent scraping for {dtos.Count} items...");
            var results = new ConcurrentBag<object>();

            await Parallel.ForEachAsync(dtos, new ParallelOptions { MaxDegreeOfParallelism = 5 }, (dto, token) =>
            {
                try
                {
                    var scraper = ScraperGenerator.Generate(dto);
                    var data = scraper.Dig();
                    if (data != null)
                    {
                        results.Add(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to process URL '{dto.Url}': {ex.Message}");
                }

                return ValueTask.CompletedTask;
            });

            if (results.IsEmpty)
            {
                Console.WriteLine("No data was successfully scraped.");
                return;
            }

            var groupedResults = results.GroupBy(r => r.ToString()).ToList();
            
            Console.WriteLine($"Scraping complete. Exporting {groupedResults.Count} categories to CSV...");

            foreach (var group in groupedResults)
            {
                CsvGenerator.Generate(group.ToList());
            }
        }
    }
}
