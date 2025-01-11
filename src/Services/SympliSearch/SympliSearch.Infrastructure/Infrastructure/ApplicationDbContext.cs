using Elastic.Clients.Elasticsearch.Security;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Role = SympliSearch.Domain.Entities.Role;
using User = SympliSearch.Domain.Entities.User;

namespace SympliSearch.Infrastructure.Infrastructure;

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

        modelBuilder.AddTransactionalOutboxEntities();
    }
}
