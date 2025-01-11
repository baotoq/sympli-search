using Microsoft.Extensions.DependencyInjection;
using SearchService.Application.Common.Interfaces;

namespace SearchService.Application.Services.Search;

public enum SearchEngineType
{
    Google,
    Bing
}

public class SearchEngineFactory: ISearchEngineFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SearchEngineFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISearchEngine Create(SearchEngineType engine)
    {
        return (engine switch
        {
            SearchEngineType.Google => _serviceProvider.GetService<GoogleSearchEngine>(),
            SearchEngineType.Bing => _serviceProvider.GetService<BingSearchEngine>(),
            _ => throw new ArgumentException($"Search engine '{engine}' not supported.")
        })!;
    }
}
