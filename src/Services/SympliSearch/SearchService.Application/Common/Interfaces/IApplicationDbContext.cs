using Microsoft.EntityFrameworkCore;
using SearchService.Domain.Entities;

namespace SearchService.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<SearchHistory> SearchHistories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
