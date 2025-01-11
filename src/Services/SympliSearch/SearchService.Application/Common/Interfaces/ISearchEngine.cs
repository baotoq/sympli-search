namespace SearchService.Application.Common.Interfaces;

public interface ISearchEngine
{
    Task<List<int>> GetSearchResultsAsync(string keywords, string url);
}
