using System.Net.Http.Headers;
using gasmie.src.models;
using gasmie.src.models.notion;
using Newtonsoft.Json;

namespace gasmie.src.services;

public static class NotionRequest
{
    private static readonly HttpClient _httpClient = new();

    public static async ValueTask<List<ScraperDto>> GetScraperDtos(string url, string id, string key)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{url}/databases/{id}/query");
        request.Headers.Add("Notion-Version", "2022-06-28");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);

        var response = await _httpClient.SendAsync(request);
        return await ExtractScraperDtos(response);
    }

    private static async Task<List<ScraperDto>> ExtractScraperDtos(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Notion API error: {response.StatusCode} - {error}");
            return new List<ScraperDto>();
        }

        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content))
        {
            Console.WriteLine("Notion API returned empty response.");
            return new List<ScraperDto>();
        }

        var databaseResponse = JsonConvert.DeserializeObject<DatabaseResponse>(content);
        if (databaseResponse?.Results is null || databaseResponse.Results.Length == 0)
        {
            Console.WriteLine("No results found in Notion database.");
            return new List<ScraperDto>();
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
                Console.WriteLine($"Skipping invalid entry: missing URL or unsupported scraper mode. (Mode: {scraperMode}, URL: {scraperUrl})");
            }
        }

        return scrapers;
    }
}
