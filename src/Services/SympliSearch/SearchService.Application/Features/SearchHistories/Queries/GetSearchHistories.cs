using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.CircuitBreaker;
using SearchService.Application.Common;
using SearchService.Application.Common.Exceptions;
using SearchService.Application.Common.Interfaces;
using SearchService.Application.Features.Seo.DomainEvents;
using SearchService.Domain.Entities;
using SearchService.Infrastructure.Services;

namespace SearchService.Application.Features.SearchHistories.Queries;

public class GetSearchHistories : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "/api/search-histories",
            [OutputCache(Duration = 15, VaryByRouteValueNames = ["keyword", "url"])]
            [EnableRateLimiting("default")]
            [Authorize]
            async (
                    [FromQuery] int? pageNumber,
                    [FromQuery] int? pageSize,
                    IMediator mediator,
                    CancellationToken cancellationToken
                )
                => TypedResults.Ok(await mediator.Send(new Query
                {
                    PageNumber = pageNumber ?? 1
                }, cancellationToken)));
    }

    public record Query : IRequest<PaginatedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<Query, PaginatedList<Response>>
    {
        public async Task<PaginatedList<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var histories = await context.SearchHistories.PaginatedListAsync(request.PageNumber, request.PageSize);

            return new PaginatedList<Response>(histories.Items.Select(h => new Response
            {
                Id = h.Id,
                Keyword = h.Keyword,
                Url = h.Url,
                Positions = h.Positions
            }).ToList(), histories.TotalCount);
        }
    }

    public class Response
    {
        public Guid Id { get; set; }
        public string Keyword { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Positions { get; set; } = string.Empty;
    }
}
