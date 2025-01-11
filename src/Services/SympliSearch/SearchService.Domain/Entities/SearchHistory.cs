using SearchService.Domain.Common;

namespace SearchService.Domain.Entities;

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

    public required Guid SearchByUserId { get; set; }
    public required User SearchByUser { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
