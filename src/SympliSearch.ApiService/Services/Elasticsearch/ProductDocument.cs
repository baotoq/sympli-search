using SympliSearch.ApiService.Domain.Entities;

namespace SympliSearch.ApiService.Services.Elasticsearch;

public record ProductDocument
{
    public const string IndexPattern = "products*";
    public const string IndexKey = "products";

    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required long RemainingStock { get; set; }

    public static ProductDocument FromDomain(Product product)
    {
        return new ProductDocument
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            RemainingStock = product.RemainingStock
        };
    }
}
