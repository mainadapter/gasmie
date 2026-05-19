using gasmie.settings;
using gasmie.src;
using gasmie.src.notion;

try
{
    var (url, id, key) = Settings.GetNotionConnectionStrings();
    var dtos = await NotionRequest.GetScraperDtos(url, id, key);

    if (dtos.Count == 0)
    {
        Console.WriteLine("No scrapers found. Exiting.");
        return;
    }

    var scrapers = ScraperGenerator.Generate(dtos);
    var listScrapers = scrapers.GroupBy(s => s.ToString()).ToList();
    var listDtos = listScrapers.Select(l => l.Select(s => s.Dig()).ToList()).ToList();
    listDtos.ForEach(l => CsvGenerator.Generate(l));

    Console.WriteLine($"Successfully processed {listDtos.Count} scraper groups.");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Fatal error: {ex.Message}");
    Environment.Exit(1);
}