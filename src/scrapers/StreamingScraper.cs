using gasmie.src.models;

namespace gasmie.src.scrapers;

public class StreamingScraper : Scraper
{
    public string Type { get; init; }

    public StreamingScraper(string url, string type) : base(url)
    {
        Type = type;
    }

    protected StreamingScraper(HtmlAgilityPack.HtmlDocument document, string url, string type) : base(document, url)
    {
        Type = type;
    }

    public override object Dig()
    {
        return new StreamingDto(
            Type,
            DigImage(),
            DigName(),
            DigEpisodes(),
            DigGenres(),
            DigDuration(),
            URL);
    }

    private string DigImage()
    {
        var imageNode = Document.DocumentNode.SelectNodes("//span[@class='title-poster title-poster--no-radius-bottom']/picture[@class='picture-comp title-poster__image']/img");
        if (imageNode is null || imageNode.Count == 0)
            return "";

        try
        {
            var url = imageNode[0].OuterHtml.Split(' ')[3]
                .Replace("\"", "")
                .Replace("data-src=", "");
            return $"{url}.jpg";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StreamingScraper] Error parsing Image for {URL}: {ex.Message}");
            return "";
        }
    }

    private string DigName()
    {
        var nameNode = Document.DocumentNode.SelectNodes("//div[@class='title-block']/div/h1");
        return nameNode?.FirstOrDefault()?.InnerText?.Trim() ?? "";
    }

    private string DigEpisodes()
    {
        if (Type.Equals("Movie"))
            return "";

        var episodesNode = Document.DocumentNode.SelectNodes("//div[@class='detail-infos__subheading']/h2[@class='detail-infos__subheading--label']");
        return episodesNode?.FirstOrDefault(e => e.InnerHtml.Contains("Episodes"))?.InnerHtml?.Trim() ?? "";
    }

    private string DigGenres()
    {
        var genresNode = Document.DocumentNode.SelectNodes("//div[@class='title-info']/div[@class='detail-infos']/div[@class='detail-infos__value']");
        if (genresNode is null || genresNode.Count < 2)
            return "";

        return genresNode[1].InnerText.Trim()
            .Replace(" ,", ",")
            .Replace("&amp;", "&");
    }

    private string DigDuration()
    {
        var durationNode = Document.DocumentNode.SelectNodes("//div[@class='title-info visible-xs visible-sm']/div[@class='detail-infos']/div[@class='detail-infos__value']");
        if (durationNode is null || durationNode.Count < 3)
            return "";

        return durationNode[2].InnerText.Trim();
    }
}
