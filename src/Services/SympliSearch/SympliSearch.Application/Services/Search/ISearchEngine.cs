namespace SympliSearch.Application.Services.Search;

public interface ISearchEngine
{
    string EngineName { get; }
    Task<List<int>> GetSearchResultsAsync(string keywords, string url);
}
