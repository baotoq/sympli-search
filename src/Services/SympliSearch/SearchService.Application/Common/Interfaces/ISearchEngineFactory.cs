using SearchService.Application.Services.Search;
using SearchService.Infrastructure.Services;

namespace SearchService.Application.Common.Interfaces;

public interface ISearchEngineFactory
{
    ISearchEngine Create(SearchEngineType engine);
}
