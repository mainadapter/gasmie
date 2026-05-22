using HtmlAgilityPack;
using gasmie.src;

namespace gasmie.tests.Helpers;

/// <summary>
/// Test-only subclass that bypasses the network call in <see cref="StreamingScraper"/>
/// by using the protected <see cref="Scraper(HtmlDocument, string)"/> constructor.
/// </summary>
internal sealed class TestableStreamingScraper : StreamingScraper
{
    public TestableStreamingScraper(HtmlDocument document, string type)
        : base(document, "https://www.allocine.fr/test", type) { }
}
