
using gasmie.src.models;

namespace gasmie.tests;

[TestClass]
public sealed class DtoTests
{
    [TestMethod]
    public void GameDto_ToString_ReturnsGames()
    {
        var dto = new GameDto("Library", "Test", "", "", "", "", "", "", "");

        Assert.AreEqual("games", dto.ToString());
    }

    [TestMethod]
    public void StreamingDto_ToString_ReturnsStreaming()
    {
        var dto = new StreamingDto("Anime", "", "Test", "", "", "", "");

        Assert.AreEqual("streaming", dto.ToString());
    }

    [TestMethod]
    public void StreamingDto_Status_ReturnsToWatch()
    {
        var dto = new StreamingDto("Movie", "", "Inception", "", "", "", "");

        Assert.AreEqual("To Watch", dto.Status);
    }

    [TestMethod]
    public void GameDto_RecordEquality_WorksCorrectly()
    {
        var dto1 = new GameDto("Library", "Game A", "img.jpg", "10h", "20h", "\"Action\"", "Dev", "2024", "url");
        var dto2 = new GameDto("Library", "Game A", "img.jpg", "10h", "20h", "\"Action\"", "Dev", "2024", "url");

        Assert.AreEqual(dto1, dto2);
    }

    [TestMethod]
    public void StreamingDto_RecordEquality_WorksCorrectly()
    {
        var dto1 = new StreamingDto("Anime", "img.jpg", "Show A", "12 Episodes", "Action", "24min", "url");
        var dto2 = new StreamingDto("Anime", "img.jpg", "Show A", "12 Episodes", "Action", "24min", "url");

        Assert.AreEqual(dto1, dto2);
    }
}
