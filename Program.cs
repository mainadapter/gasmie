using gasmie.settings;
using gasmie.src.services;

try
{
    var (url, id, key) = Settings.GetNotionConnectionStrings();
    var dtos = await NotionRequest.GetScraperDtos(url, id, key);

    if (dtos.Count == 0)
    {
        Console.WriteLine("No scrapers found. Exiting.");
        return;
    }

    await ScrapingWorkflow.RunAsync(dtos);
    Console.WriteLine("All done!");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Fatal error: {ex.Message}");
    Environment.Exit(1);
}