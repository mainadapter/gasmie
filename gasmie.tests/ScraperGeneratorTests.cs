using gasmie.src;

namespace gasmie.tests;

[TestClass]
public sealed class ScraperGeneratorTests
{
    [TestMethod]
    public void Generate_EmptyList_ReturnsEmptyList()
    {
        var result = ScraperGenerator.Generate([]);

        Assert.IsEmpty(result);
    }

    [TestMethod]
    public void Generate_NoneMode_ThrowsArgumentException()
    {
        var dtos = new List<ScraperDto> { new("https://example.com", ScraperMode.NONE) };

        Assert.ThrowsExactly<ArgumentException>(() => ScraperGenerator.Generate(dtos));
    }
}
