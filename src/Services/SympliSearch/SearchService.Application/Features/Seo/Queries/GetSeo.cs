using System.Net.Mime;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using SearchService.Application.Common;
using SearchService.Application.Common.Exceptions;
using SearchService.Application.Common.Interfaces;
using SearchService.Application.Features.Seo.DomainEvents;
using SearchService.Application.Services.Search;
using SearchService.Domain.Entities;

namespace SearchService.Application.Features.Seo.Queries;

public class GetSeo : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "/api/seo/{searchEngineType}",
            [OutputCache(Duration = 3600, VaryByRouteValueNames = ["keyword", "url"])]
            [EnableRateLimiting("default")]
            async (
                    [FromQuery] string keyword,
                    [FromQuery] string url,
                    [FromRoute] SearchEngineType searchEngineType,
                    IMediator mediator,
                    CancellationToken cancellationToken
                )
                => TypedResults.Ok(await mediator.Send(new Query
                {
                    Keyword = keyword,
                    Url = url,
                    SearchEngineType = searchEngineType
                }, cancellationToken)));
    }

    public record Query : IRequest<string>
    {
        public required string Keyword { get; init; }
        public required string Url { get; init; }
        public SearchEngineType SearchEngineType { get; set; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(q => q.Keyword).NotEmpty();
            RuleFor(q => q.Url).NotEmpty();
            RuleFor(q => q.SearchEngineType).IsInEnum();
        }
    }

    public class Handler(IApplicationDbContext context, ISearchEngineFactory searchEngineFactory) : IRequestHandler<Query, string>
    {
        private static readonly AsyncCircuitBreakerPolicy s_circuitBreaker
            = Policy.Handle<Exception>().CircuitBreakerAsync(3, TimeSpan.FromMinutes(1));

        public async Task<string> Handle(Query request, CancellationToken cancellationToken)
        {
            if (s_circuitBreaker.CircuitState == CircuitState.Open)
            {
                throw new ServiceUnavailableException();
            }

            var searchEngine = searchEngineFactory.Create(request.SearchEngineType);

            var results =
                s_circuitBreaker.ExecuteAsync(
                    () => searchEngine.GetSearchResultsAsync(request.Keyword, request.Url));

            var searchHistory = new SearchHistory
            {
                Keyword = request.Keyword,
                Url = request.Url,
                Positions = results.ToString(),
                SearchByUserId = Guid.NewGuid()
            };

            searchHistory.AddDomainEvent(new GetSeoSuccessfullyDomainEvent
            {
                Keyword = request.Keyword,
                Url = request.Url,
                Positions = results.ToString(),
                SearchByUserId = Guid.NewGuid()
            });

            await context.SearchHistories.AddAsync(searchHistory, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return request.Url;
        }
    }
}
