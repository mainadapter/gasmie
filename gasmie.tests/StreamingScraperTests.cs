using gasmie.src.models;
using gasmie.tests.helpers;

namespace gasmie.tests;

[TestClass]
public sealed class StreamingScraperTests
{
    /// <summary>
    /// Builds an HTML fixture mimicking an AlloCiné page structure.
    /// </summary>
    private static string BuildStreamingHtml(
        string? image = null,
        string? name = null,
        string? episodes = null,
        string? genresAndDuration = null)
    {
        var html = "<html><body>";

        if (image is not null)
            html += image;

        if (name is not null)
            html += name;

        if (episodes is not null)
            html += episodes;

        if (genresAndDuration is not null)
            html += genresAndDuration;

        html += "</body></html>";
        return html;
    }

    private static string ImageBlock(string url) =>
        "<span class=\"title-poster title-poster--no-radius-bottom\">" +
        "<picture class=\"picture-comp title-poster__image\">" +
        $"<img loading=\"lazy\" class=\"\" data-src=\"{url}\" alt=\"poster\" />" +
        "</picture></span>";

    private static string NameBlock(string name) =>
        $"<div class=\"title-block\"><div><h1>{name}</h1></div></div>";

    private static string EpisodesBlock(string episodes) =>
        "<div class=\"detail-infos__subheading\">" +
        $"<h2 class=\"detail-infos__subheading--label\">{episodes}</h2>" +
        "</div>";

    private static string GenresAndDurationBlock(string genres, string duration) =>
        "<div class=\"title-info\">" +
        "<div class=\"detail-infos\"><div class=\"detail-infos__value\">Some other info</div></div>" +
        $"<div class=\"detail-infos\"><div class=\"detail-infos__value\">{genres}</div></div>" +
        "<div class=\"detail-infos\"><div class=\"detail-infos__value\">More info</div></div>" +
        "</div>";

    private static StreamingDto DigStreaming(string html, string type = "Anime")
    {
        var doc = HtmlFixture.FromHtml(html);
        var scraper = new TestableStreamingScraper(doc, type);
        return (StreamingDto)scraper.Dig();
    }

    // ====================================================================
    // Happy Path
    // ====================================================================

    [TestMethod]
    public void Dig_FullHtml_ReturnsCompleteStreamingDto()
    {
        var html = BuildStreamingHtml(
            image: ImageBlock("https://images.com/poster"),
            name: NameBlock("Attack on Titan"),
            episodes: EpisodesBlock("87 Episodes"),
            genresAndDuration: GenresAndDurationBlock("Action , Animation &amp; Fantasy", "24min"));

        var dto = DigStreaming(html, "Anime");

        Assert.AreEqual("Anime", dto.Type);
        Assert.AreEqual("Attack on Titan", dto.Name);
        Assert.AreEqual("https://images.com/poster.jpg", dto.Image);
        Assert.AreEqual("87 Episodes", dto.Episodes);
        Assert.AreEqual("Action, Animation & Fantasy", dto.Genres);
        Assert.AreEqual("To Watch", dto.Status);
    }

    // ====================================================================
    // Movie Type
    // ====================================================================

    [TestMethod]
    public void Dig_MovieType_EpisodesEmpty()
    {
        var html = BuildStreamingHtml(
            name: NameBlock("Inception"),
            episodes: EpisodesBlock("Should Be Ignored"));

        var dto = DigStreaming(html, "Movie");

        Assert.AreEqual("", dto.Episodes);
    }

    // ====================================================================
    // Missing Nodes
    // ====================================================================

    [TestMethod]
    public void Dig_MissingAllNodes_ReturnsEmptyStrings()
    {
        var html = "<html><body></body></html>";

        var dto = DigStreaming(html);

        Assert.AreEqual("", dto.Name);
        Assert.AreEqual("", dto.Image);
        Assert.AreEqual("", dto.Episodes);
        Assert.AreEqual("", dto.Genres);
        Assert.AreEqual("", dto.Duration);
    }

    [TestMethod]
    public void Dig_MissingImageNode_ReturnsEmpty()
    {
        var html = BuildStreamingHtml(
            name: NameBlock("Some Show"));

        var dto = DigStreaming(html);

        Assert.AreEqual("", dto.Image);
        Assert.AreEqual("Some Show", dto.Name);
    }

    [TestMethod]
    public void Dig_MissingNameNode_ReturnsEmpty()
    {
        var html = BuildStreamingHtml(
            image: ImageBlock("https://images.com/poster"));

        var dto = DigStreaming(html);

        Assert.AreEqual("", dto.Name);
    }
}
