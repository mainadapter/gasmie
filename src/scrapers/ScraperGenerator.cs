using gasmie.src.models;

namespace gasmie.src.scrapers;

public class ScraperGenerator
{
    private ScraperGenerator() { }
    public static List<Scraper> Generate(List<ScraperDto> dtos)
    {
        return [.. dtos.Select(Generate)];
    }

    public static Scraper Generate(ScraperDto dto)
    {
        return dto.Mode switch
        {
            ScraperMode.ANIME => new StreamingScraper(dto.Url, "Anime"),
            ScraperMode.GAME => new GameScraper(dto.Url),
            ScraperMode.MOVIE => new StreamingScraper(dto.Url, "Movie"),
            ScraperMode.TVSERIES => new StreamingScraper(dto.Url, "TV Series"),
            _ => throw new ArgumentException("Invalid Mode"),
        };
    }
}
