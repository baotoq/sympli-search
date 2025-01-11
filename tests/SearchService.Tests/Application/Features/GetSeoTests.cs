using NSubstitute;
using SearchService.Application.Common.Interfaces;
using SearchService.Application.Features.Seo.Queries;
using SearchService.Application.Services.Search;

namespace SearchService.Tests.Application.Features;

public class GetSeoTests : TestBase
{
    [Fact]
    public async Task ShouldGetSeo()
    {
        // Arrange
        await SeedContext.SaveChangesAsync();

        var mockSearchEngineFactory = Substitute.For<ISearchEngineFactory>();
        var mockSearchEngine = Substitute.For<ISearchEngine>();

        var query = new GetSeo.Query
        {
            Keyword = "test",
            Url = "http://example.com",
            SearchEngineType = SearchEngineType.Google
        };

        var searchResults = new List<int> { 1, 2, 3 };
        mockSearchEngine.GetSearchResultsAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(searchResults);
        mockSearchEngineFactory.Create(query.SearchEngineType).Returns(mockSearchEngine);

        var sut = new GetSeo.Handler(SeedContext, mockSearchEngineFactory);

        // Act
        var act = await sut.Handle(new GetSeo.Query
        {
            Keyword = "sympli",
            Url = "https://www.sympli.com.au",
            SearchEngineType = SearchEngineType.Google
        }, CancellationToken.None);

        // Assert
        Verify(act);
    }
}
