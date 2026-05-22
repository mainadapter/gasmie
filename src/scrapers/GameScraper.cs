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
        protected GameScraper(HtmlDocument document, string url) : base(document, url) { }

        public override object Dig()
        {
            var (name, image) = DigNameAndImage();

            return new GameDto(
                "Library",
                name,
                image,
                DigDuration("Main + Sides"),
                DigDuration("Completionist"),
                DigGenres(),
                DigDevelopers(),
                DigRelease(),
                URL);
        }

        private (string name, string image) DigNameAndImage()
        {
            var nameAndImageNode = Document.DocumentNode.SelectNodes(NAME_AND_IMAGE_NODE);
            if (nameAndImageNode is null || nameAndImageNode.Count == 0)
                return ("", "");

            try
            {
                var outerHtml = nameAndImageNode.First().OuterHtml.Split("\"");
                var name = FormatString(outerHtml[1], ["Box Art"]).Replace("&#x27;", "'");
                var image = outerHtml[3].Split("?")[0];
                return (name, image);
            }
            catch
            {
                return ("", "");
            }
        }

        private string DigDuration(string keyword)
        {
            var nodes = Document.DocumentNode.SelectNodes(DURATION_NODE);
            var durationNode = nodes?.FirstOrDefault(n => n.InnerHtml.Contains(keyword));
            
            return durationNode switch
            {
                null => "",
                _ => FormatString(durationNode.InnerText, [keyword, "\t"]).Replace("&#189;", "½")
            };
        }

        private string DigGenres()
        {
            var nodes = Document.DocumentNode.SelectNodes(GENRES_AND_DEVELOPERS_NODE);
            var genresNode = FindNodeByKeyword(nodes, "Genre");
            if (genresNode is null)
                return "";

            var raw = FormatString(genresNode.InnerText, ["Genres:", "Genre:", "\n", "\t"]);
            var genres = raw.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return string.Join(", ", genres.Select(g => $"\"{g}\""));
        }

        private string DigDevelopers()
        {
            var nodes = Document.DocumentNode.SelectNodes(GENRES_AND_DEVELOPERS_NODE);
            var developersNode = FindNodeByKeyword(nodes, "Developer");
            return developersNode switch
            {
                null => "",
                _ => FormatString(developersNode.InnerText, ["Developers:", "Developer:", "\n", "\t"])
            };
        }

        private string DigRelease()
        {
            var releaseNode = Document.DocumentNode.SelectNodes(RELEASE_NODE);
            if (releaseNode is null || !releaseNode.Any())
                return "";

            try
            {
                var dateString = FormatString(releaseNode.First().InnerText, ["\n", "\t"]).Split(':')[1].Trim();
                return System.Text.RegularExpressions.Regex.Replace(dateString, @"(\d+)(st|nd|rd|th)", "$1");
            }
            catch
            {
                return "";
            }
        }

        private static HtmlNode? FindNodeByKeyword(HtmlNodeCollection? nodes, string keyword)
        {
            return nodes?.FirstOrDefault(n => n.InnerText.Contains(keyword));
        }

        private static string FormatString(string value, params string[] tokens)
        {
            foreach (var token in tokens)
            {
                value = value.Replace(token, "");
            }
            return value.Trim();
        }
    }
}