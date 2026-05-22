using HtmlAgilityPack;

namespace gasmie.tests.Helpers;

/// <summary>
/// Utility to load <see cref="HtmlDocument"/> from raw HTML strings.
/// </summary>
internal static class HtmlFixture
{
    public static HtmlDocument FromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc;
    }
}
