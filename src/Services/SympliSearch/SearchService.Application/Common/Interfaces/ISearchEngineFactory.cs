using SearchService.Application.Services.Search;

namespace SearchService.Application.Common.Interfaces;

public interface ISearchEngineFactory
{
    ISearchEngine Create(SearchEngineType engine);
}
