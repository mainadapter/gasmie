using HtmlAgilityPack;
using gasmie.src;

namespace gasmie.tests.Helpers;

/// <summary>
/// Test-only subclass that bypasses the network call in <see cref="GameScraper"/>
/// by using the protected <see cref="Scraper(HtmlDocument, string)"/> constructor.
/// </summary>
internal sealed class TestableGameScraper : GameScraper
{
    public TestableGameScraper(HtmlDocument document)
        : base(document, "https://howlongtobeat.com/game/test") { }
}
