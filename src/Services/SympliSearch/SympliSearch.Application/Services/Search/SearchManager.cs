namespace SympliSearch.Application.Services.Search;

public class SearchManager
{
    private readonly SearchEngineFactory _searchEngineFactory;
    private readonly ICacheService _cacheService;

    public SearchManager(CacheService cacheService, SearchEngineFactory searchEngineFactory)
    {
        _cacheService = cacheService;
        _searchEngineFactory = searchEngineFactory;
    }

    public async Task<List<int>> SearchAsync(string keywords, string url, SearchEngineType engine)
    {
        string cacheKey = $"{engine}:{keywords}";

        var cachedResults = await _cacheService.GetAsync<List<int>>(cacheKey);
        if (cachedResults is not null)
        {
            return cachedResults;
        }

        var searchEngine = _searchEngineFactory.Create(engine);

        var results = await searchEngine.GetSearchResultsAsync(keywords, url);
        await _cacheService.SetAsync(cacheKey, results, TimeSpan.FromHours(1));

        return results;
    }
}
