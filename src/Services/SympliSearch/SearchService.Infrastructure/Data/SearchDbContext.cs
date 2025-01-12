using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SearchService.Application.Common.Interfaces;
using SearchService.Domain.Entities;

namespace SearchService.Infrastructure.Data;

public class SearchDbContext : DbContext, IApplicationDbContext
{
    public SearchDbContext()
    {
    }

    public SearchDbContext(DbContextOptions<SearchDbContext> options) : base(options)
    {
    }

    public DbSet<SearchHistory> SearchHistories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SearchHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Keyword).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Positions).IsRequired();
            entity.Property(e => e.SearchByUserId).IsRequired();
            entity.HasIndex(e => e.SearchByUserId);
        });

        modelBuilder.AddTransactionalOutboxEntities();
    }
}
