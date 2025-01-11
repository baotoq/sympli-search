using SympliSearch.ApiService.Domain.Common;

namespace SympliSearch.Domain.Entities;

public class SearchHistory : IDateEntity
{
    public SearchHistory()
    {
        CreatedAt = DateTimeOffset.UtcNow;
        Id = Guid.CreateVersion7(CreatedAt);
    }

    public Guid Id { get; set; }
    public string Keyword { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Positions { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
