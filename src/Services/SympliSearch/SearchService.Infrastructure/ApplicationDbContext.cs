using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SearchService.Domain.Entities;
using Role = SearchService.Domain.Entities.Role;
using User = SearchService.Domain.Entities.User;

namespace SearchService.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SearchHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Keyword).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Positions).IsRequired();
            entity
                .HasOne(e => e.SearchByUser)
                .WithOne()
                .HasForeignKey<SearchHistory>(e => e.SearchByUserId)
                .IsRequired();
        });

        modelBuilder.AddTransactionalOutboxEntities();
    }
}
