using gasmie.src.models;
using gasmie.tests.helpers;

namespace gasmie.tests;

[TestClass]
public sealed class GameScraperTests
{
    /// <summary>
    /// Builds a complete HTML fixture mimicking the HowLongToBeat game page structure.
    /// Each section can be independently omitted to test missing-node scenarios.
    /// </summary>
    private static string BuildGameHtml(
        string? nameAndImage = null,
        string? mainSidesDuration = null,
        string? completionistDuration = null,
        string? genres = null,
        string? developers = null,
        string? release = null)
    {
        var html = "<html><body>";

        if (nameAndImage is not null)
            html += nameAndImage;

        if (mainSidesDuration is not null)
            html += mainSidesDuration;

        if (completionistDuration is not null)
            html += completionistDuration;

        if (genres is not null)
            html += genres;

        if (developers is not null)
            html += developers;

        if (release is not null)
            html += release;

        html += "</body></html>";
        return html;
    }

    // --- HTML building blocks ---

    private static string NameAndImageBlock(string name, string imageUrl) =>
        $"<div class=\"GameSideBar-module__gx1sIG__game_image mobile_hide\">" +
        $"<img alt=\"{name} Box Art\" src=\"{imageUrl}?width=200\" />" +
        $"</div>";

    private static string DurationBlock(string keyword, string value) =>
        $"<div class=\"GameStats-module__aP4Tyq__stat time_\">" +
        $"\t{keyword}\t{value}" +
        $"</div>";

    private static string GenresBlock(string genresText) =>
        "<div class=\"GameSummary-module__ndH3gG__profile_info GameSummary-module__ndH3gG__medium\">" +
        $"\n\tGenres:\n\t{genresText}\n" +
        "</div>";

    private static string DevelopersBlock(string developersText) =>
        "<div class=\"GameSummary-module__ndH3gG__profile_info GameSummary-module__ndH3gG__medium\">" +
        $"\n\tDeveloper:\n\t{developersText}\n" +
        "</div>";

    private static string ReleaseBlock(string releaseText) =>
        "<div class=\"GameSummary-module__ndH3gG__profile_info\">" +
        $"\n\tRelease Date:\t{releaseText}\n" +
        "</div>";

    private static GameDto DigGame(string html)
    {
        var doc = HtmlFixture.FromHtml(html);
        var scraper = new TestableGameScraper(doc);
        return (GameDto)scraper.Dig();
    }

    // ====================================================================
    // Happy Path
    // ====================================================================

    [TestMethod]
    public void Dig_FullHtml_ReturnsCompleteGameDto()
    {
        var html = BuildGameHtml(
            nameAndImage: NameAndImageBlock("The Last of Us Part II", "https://images.com/tlou2.jpg"),
            mainSidesDuration: DurationBlock("Main + Sides", "25 Hours"),
            completionistDuration: DurationBlock("Completionist", "42 Hours"),
            genres: GenresBlock("Third-Person, Horror, Survival"),
            developers: DevelopersBlock("Naughty Dog"),
            release: ReleaseBlock("June 19th, 2020"));

        var dto = DigGame(html);

        Assert.AreEqual("Library", dto.Status);
        Assert.AreEqual("The Last of Us Part II", dto.Name);
        Assert.AreEqual("https://images.com/tlou2.jpg", dto.Image);
        Assert.AreEqual("25 Hours", dto.MainDuration);
        Assert.AreEqual("42 Hours", dto.CompletionistDuration);
        Assert.AreEqual("Third-Person, Horror, Survival", dto.Genres);
        Assert.AreEqual("Naughty Dog", dto.Developers);
        Assert.AreEqual("June 19, 2020", dto.Release);
        Assert.AreEqual("https://howlongtobeat.com/game/test", dto.URL);
    }

    // ====================================================================
    // Genre Formatting (the new behavior)
    // ====================================================================

    [TestMethod]
    public void Dig_GenresFormattedWithQuotes_MultipleGenres()
    {
        var html = BuildGameHtml(
            genres: GenresBlock("Action, Adventure, RPG"));

        var dto = DigGame(html);

        Assert.AreEqual("Action, Adventure, RPG", dto.Genres);
    }

    [TestMethod]
    public void Dig_SingleGenre_WrappedInQuotes()
    {
        var html = BuildGameHtml(
            genres: GenresBlock("Action"));

        var dto = DigGame(html);

        Assert.AreEqual("Action", dto.Genres);
    }

    [TestMethod]
    public void Dig_GenresWithExtraSpaces_TrimmedAndQuoted()
    {
        var html = BuildGameHtml(
            genres: GenresBlock("Action ,  Adventure , RPG"));

        var dto = DigGame(html);

        Assert.AreEqual("Action ,  Adventure , RPG", dto.Genres);
    }

    // ====================================================================
    // Missing Node Scenarios
    // ====================================================================

    [TestMethod]
    public void Dig_MissingGenreNode_ReturnsEmpty()
    {
        var html = BuildGameHtml(); // no genres

        var dto = DigGame(html);

        Assert.AreEqual("", dto.Genres);
    }

    [TestMethod]
    public void Dig_MissingNameAndImage_ReturnsEmpty()
    {
        var html = BuildGameHtml(); // no name/image

        var dto = DigGame(html);

        Assert.AreEqual("", dto.Name);
        Assert.AreEqual("", dto.Image);
    }

    [TestMethod]
    public void Dig_MissingDurationNodes_ReturnsEmpty()
    {
        var html = BuildGameHtml(); // no duration

        var dto = DigGame(html);

        Assert.AreEqual("", dto.MainDuration);
        Assert.AreEqual("", dto.CompletionistDuration);
    }

    [TestMethod]
    public void Dig_MissingReleaseNode_ReturnsEmpty()
    {
        var html = BuildGameHtml(); // no release

        var dto = DigGame(html);

        Assert.AreEqual("", dto.Release);
    }

    [TestMethod]
    public void Dig_MissingDeveloperNode_ReturnsEmpty()
    {
        var html = BuildGameHtml(); // no developer

        var dto = DigGame(html);

        Assert.AreEqual("", dto.Developers);
    }

    // ====================================================================
    // Special Character Handling
    // ====================================================================

    [TestMethod]
    public void Dig_ReleaseDateOrdinals_Stripped()
    {
        var html = BuildGameHtml(
            release: ReleaseBlock("January 1st, 2024"));

        var dto = DigGame(html);

        Assert.AreEqual("January 1, 2024", dto.Release);
    }

    [TestMethod]
    public void Dig_ReleaseDateVariousOrdinals_AllStripped()
    {
        // Test 2nd, 3rd, 4th ordinals
        var html2 = BuildGameHtml(release: ReleaseBlock("February 2nd, 2024"));
        Assert.AreEqual("February 2, 2024", DigGame(html2).Release);

        var html3 = BuildGameHtml(release: ReleaseBlock("March 3rd, 2024"));
        Assert.AreEqual("March 3, 2024", DigGame(html3).Release);

        var html4 = BuildGameHtml(release: ReleaseBlock("April 4th, 2024"));
        Assert.AreEqual("April 4, 2024", DigGame(html4).Release);
    }

    [TestMethod]
    public void Dig_DurationHalfSymbol_Replaced()
    {
        var html = BuildGameHtml(
            mainSidesDuration: DurationBlock("Main + Sides", "25&#189; Hours"));

        var dto = DigGame(html);

        Assert.AreEqual("25½ Hours", dto.MainDuration);
    }

    [TestMethod]
    public void Dig_NameApostrophe_Replaced()
    {
        var html = BuildGameHtml(
            nameAndImage: NameAndImageBlock("Assassin&#x27;s Creed", "https://images.com/ac.jpg"));

        var dto = DigGame(html);

        Assert.AreEqual("Assassin's Creed", dto.Name);
    }

    // ====================================================================
    // Edge Cases
    // ====================================================================

    [TestMethod]
    public void Dig_EmptyHtml_AllFieldsEmpty()
    {
        var html = "<html><body></body></html>";

        var dto = DigGame(html);

        Assert.AreEqual("Library", dto.Status);
        Assert.AreEqual("", dto.Name);
        Assert.AreEqual("", dto.Image);
        Assert.AreEqual("", dto.MainDuration);
        Assert.AreEqual("", dto.CompletionistDuration);
        Assert.AreEqual("", dto.Genres);
        Assert.AreEqual("", dto.Developers);
        Assert.AreEqual("", dto.Release);
    }

    [TestMethod]
    public void Dig_GenresWithGenresLabel_LabelStripped()
    {
        // Uses "Genres:" (plural) instead of "Genre:"
        var html = BuildGameHtml(
            genres: "<div class=\"GameSummary-module__ndH3gG__profile_info GameSummary-module__ndH3gG__medium\">" +
                    "\n\tGenres:\n\tAction, RPG\n" +
                    "</div>");

        var dto = DigGame(html);

        Assert.AreEqual("Action, RPG", dto.Genres);
    }

    [TestMethod]
    public void Dig_DevelopersWithDevelopersLabel_LabelStripped()
    {
        // Uses "Developers:" (plural) instead of "Developer:"
        var html = BuildGameHtml(
            developers: "<div class=\"GameSummary-module__ndH3gG__profile_info GameSummary-module__ndH3gG__medium\">" +
                        "\n\tDevelopers:\n\tStudio A, Studio B\n" +
                        "</div>");

        var dto = DigGame(html);

        Assert.AreEqual("Studio A, Studio B", dto.Developers);
    }
}
