using HtmlAgilityPack;

namespace gasmie.src.scrapers;

public abstract class Scraper
{
    public HtmlDocument Document { get; }
    public string URL { get; }

    protected Scraper(string url)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        URL = url;
        var web = new HtmlWeb();
        Document = web.Load(url);
    }

    protected Scraper(HtmlDocument document, string url)
    {
        URL = url;
        Document = document;
    }

    public abstract object Dig();
}
