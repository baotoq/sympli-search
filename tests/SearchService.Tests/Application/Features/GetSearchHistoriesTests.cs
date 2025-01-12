using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using SearchService.Application.Common;
using SearchService.Application.Common.Interfaces;
using SearchService.Application.Features.SearchHistories.Queries;
using SearchService.Domain.Entities;
using Xunit;

namespace SearchService.Tests.Application.Features
{
    public class GetSearchHistoriesTests : TestBase
    {
        [Fact]
        public async Task Handle_ShouldReturnPaginatedList_WhenQueryIsValid()
        {
            // Arrange
            var searchHistories = new List<SearchHistory>
            {
                new SearchHistory
                {
                    Id = Guid.NewGuid(),
                    Keyword = "test1",
                    Url = "http://example1.com",
                    Positions = "1,2,3"
                },
                new SearchHistory
                {
                    Id = Guid.NewGuid(),
                    Keyword = "test2",
                    Url = "http://example2.com",
                    Positions = "4,5,6"
                }
            };

            await SeedContext.SearchHistories.AddRangeAsync(searchHistories);
            await SeedContext.SaveChangesAsync();

            var handler = new GetSearchHistories.Handler(SeedContext);

            var query = new GetSearchHistories.Query
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Verify(result);
        }
    }
}
