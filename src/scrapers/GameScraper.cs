using HtmlAgilityPack;

namespace gasmie.src
{
    public class GameScraper : Scraper
    {
        private const string NAME_AND_IMAGE_CODE = "gx1sIG";
        private static readonly string NAME_AND_IMAGE_NODE = $"//div[@class='GameSideBar-module__{NAME_AND_IMAGE_CODE}__game_image mobile_hide']/img";
        private const string DURATION_CODE = "aP4Tyq";
        private static readonly string DURATION_NODE = $"//div[contains(@class, 'GameStats-module__{DURATION_CODE}__stat') and contains(@class, 'time_')]";
        private const string GENRES_AND_DEVELOPERS_CODE = "ndH3gG";
        private static readonly string GENRES_AND_DEVELOPERS_NODE = $"//div[@class='GameSummary-module__{GENRES_AND_DEVELOPERS_CODE}__profile_info GameSummary-module__{GENRES_AND_DEVELOPERS_CODE}__medium']";
        private const string RELEASE_CODE = "ndH3gG";
        private static readonly string RELEASE_NODE = $"//div[@class='GameSummary-module__{RELEASE_CODE}__profile_info']";

        public GameScraper(string url) : base(url) { }

        public override object Dig()
        {
            var nameAndImage = DigNameAndImage();

            return new GameDto(
                "Library",
                nameAndImage[0],
                nameAndImage[1],
                DigDuration("Main + Sides"),
                DigDuration("Completionist"),
                DigGenres(),
                DigDevelopers(),
                DigRelease(),
                URL);
        }

        private string[] DigNameAndImage()
        {
            string[] nameAndImage = { "", "" };

            var nameAndImageNode = Document.DocumentNode.SelectNodes(NAME_AND_IMAGE_NODE);
            if (nameAndImageNode is not null)
            {
                var outerHtml = nameAndImageNode.First().OuterHtml.Split("\"");
                nameAndImage[0] = FormatString(outerHtml[1], new string[] { "Box Art" }).Replace("&#x27;", "'");
                nameAndImage[1] = outerHtml[3];
            }

            return nameAndImage;
        }

        private string DigDuration(string keyword)
        {
            var nodes = Document.DocumentNode.SelectNodes(DURATION_NODE);
            var durationNode = nodes.FirstOrDefault(n => n.InnerHtml.Contains(keyword));
            return durationNode is null ? "" : FormatString(durationNode.InnerText, [keyword, "\t"]).Replace("&#189;", "½");
        }

        private string DigGenres()
        {
            var genresNode = DigNodeByKeyword(Document.DocumentNode.SelectNodes(GENRES_AND_DEVELOPERS_NODE), "Genre");
            return genresNode is null ? "" : FormatString(genresNode.InnerText, new string[] { "Genres:", "Genre:", "\n", "\t" });
        }

        private string DigDevelopers()
        {
            var developersNode = DigNodeByKeyword(Document.DocumentNode.SelectNodes(GENRES_AND_DEVELOPERS_NODE), "Developer");
            return developersNode is null ? "" : FormatString(developersNode.InnerText, new string[] { "Developers:", "Developer:", "\n", "\t" });
        }

        private string DigRelease()
        {
            var releaseNode = Document.DocumentNode.SelectNodes(RELEASE_NODE);
            if (releaseNode is null) return "";
            
            var dateString = FormatString(releaseNode.First().InnerText, new string[] { "\n", "\t" }).Split(":")[1].Trim();
            return System.Text.RegularExpressions.Regex.Replace(dateString, @"(\d+)(st|nd|rd|th)", "$1");
        }

        private static HtmlNode? DigNodeByKeyword(HtmlNodeCollection nodes, string keyword)
        {
            return nodes.FirstOrDefault(n => n.InnerText.Contains(keyword));
        }

        private static string FormatString(string value, string[] strings)
        {
            foreach (var str in strings)
            {
                value = value.Replace(str, "");
            }

            return value.Trim();
        }
    }
}