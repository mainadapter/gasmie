using Newtonsoft.Json;
using RestSharp;

namespace gasmie.src.notion
{
    public static class NotionRequest
    {
        public static async ValueTask<List<ScraperDto>> GetScraperDtos(string url, string id, string key)
        {
            using var client = new RestClient(url);
            var request = new RestRequest($"databases/{id}/query", Method.Post);
            request.AddHeader("Notion-Version", "2022-06-28");
            request.AddHeader("Authorization", $"Bearer {key}");

            var response = await client.ExecuteAsync(request);
            return ExtractScraperDtos(response);
        }

        private static List<ScraperDto> ExtractScraperDtos(RestResponse response)
        {
            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Notion API error: {response.ErrorMessage}");
                return [];
            }

            if (string.IsNullOrEmpty(response.Content))
            {
                Console.WriteLine("Notion API returned empty response.");
                return [];
            }

            var databaseResponse = JsonConvert.DeserializeObject<DatabaseResponse>(response.Content);
            if (databaseResponse?.Results is null || databaseResponse.Results.Length == 0)
            {
                Console.WriteLine("No results found in Notion database.");
                return [];
            }

            var scrapers = new List<ScraperDto>();
            foreach (var result in databaseResponse.Results)
            {
                var scraperUrl = result?.Properties?.Link?.Url;
                var scraperMode = result?.Properties?.Type?.Select?.Name;

                if (scraperMode is ScraperMode mode && mode != ScraperMode.NONE && !string.IsNullOrEmpty(scraperUrl))
                {
                    scrapers.Add(new ScraperDto(scraperUrl, mode));
                }
                else
                {
                    Console.WriteLine("Skipping invalid entry: missing URL or unsupported scraper mode.");
                }
            }

            return scrapers;
        }
    }
}