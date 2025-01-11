namespace SearchService.Application.Services.Search;

public interface ISearchEngine
{
    Task<List<int>> GetSearchResultsAsync(string keywords, string url);
}
