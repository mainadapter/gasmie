using HtmlAgilityPack;
using gasmie.src.scrapers;

namespace gasmie.tests.helpers;

/// <summary>
/// Test-only subclass that bypasses the network call in <see cref="StreamingScraper"/>
/// by using the protected <see cref="Scraper(HtmlDocument, string)"/> constructor.
/// </summary>
internal sealed class TestableStreamingScraper : StreamingScraper
{
    public TestableStreamingScraper(HtmlDocument document, string type)
        : base(document, "https://www.justwatch.com/br/test", type) { }
}
