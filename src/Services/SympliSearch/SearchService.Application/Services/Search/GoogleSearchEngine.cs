using System.Text.RegularExpressions;
using SearchService.Application.Common.Interfaces;

namespace SearchService.Application.Services.Search;

public class GoogleSearchEngine : ISearchEngine
{
    public async Task<List<int>> GetSearchResultsAsync(string keywords, string url)
    {
        string query = $"https://www.google.com/search?q={Uri.EscapeDataString(keywords)}";
        string html = await FetchHtmlAsync(query);
        return ParseResults(html, url);
    }

    private async Task<string> FetchHtmlAsync(string url)
    {
        using var client = new HttpClient();
        return await client.GetStringAsync(url);
    }

    private List<int> ParseResults(string html, string targetUrl)
    {
        var matches = Regex.Matches(html, @"<a\s[^>]*href=['""](https?://[^'""]+)", RegexOptions.IgnoreCase);
        var positions = new List<int>();

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].Groups[1].Value.Contains(targetUrl, StringComparison.OrdinalIgnoreCase))
            {
                positions.Add(i + 1);
            }
        }

        return positions.Count > 0 ? positions : new List<int> { 0 };
    }
}
