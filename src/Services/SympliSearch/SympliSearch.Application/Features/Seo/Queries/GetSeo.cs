using System.IO.Compression;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Routing;
using SympliSearch.Infrastructure.Infrastructure;

namespace SympliSearch.Application.Features.Seo.Queries;

public class GetSeo : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/products/images/{url}",
            [OutputCache(Duration = 15, VaryByRouteValueNames = ["url"])]
            async (string url) =>
            {
                return TypedResults.File(new MemoryStream(), MediaTypeNames.Image.Jpeg);
            }).Produces<Stream>(contentType: MediaTypeNames.Image.Jpeg);
    }
}
