using System.Text.RegularExpressions;
using SearchService.Application.Common.Interfaces;

namespace SearchService.Application.Services.Search;

public class GoogleSearchEngine : ISearchEngine
{
    private readonly HttpClient _httpClient;

    public GoogleSearchEngine(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("https://www.google.com.au");

        _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
    }

    public async Task<List<int>> GetSearchResultsAsync(string keywords, string url)
    {
        var htmlContent = await SearchGoogleAsync(keywords);

        var foundUrls = ParseHtml(htmlContent);

        var positions = new List<int>();
        int position = 1;
        foreach (var foundUrl in foundUrls)
        {
            if (foundUrl.Contains(url, StringComparison.OrdinalIgnoreCase))
            {
                positions.Add(position);
            }
            position++;
        }

        return positions.Count > 0 ? positions : [0];
    }

    private async Task<string> SearchGoogleAsync(string keywords)
    {
        var url = $"/search?q={Uri.EscapeDataString(keywords)}&num=100";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private List<string> ParseHtml(string html)
    {
        // Basic HTML parsing logic to extract URLs
        var urls = new List<string>();
        var regex = new Regex(@"(https://[^""]+)", RegexOptions.IgnoreCase);
        foreach (Match match in regex.Matches(html).Take(100))
        {
            urls.Add(match.Groups[1].Value);
        }
        return urls;
    }
}
