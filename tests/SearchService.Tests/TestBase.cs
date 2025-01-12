using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SearchService.Infrastructure.Data;
using Testcontainers.PostgreSql;

namespace SearchService.Tests;

public abstract class TestBase : IAsyncLifetime
{
    protected SearchDbContext SeedContext { get; private set; } = null!;
    protected SearchDbContext VerifyContext { get; private set; } = null!;

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var contextOptions = new DbContextOptionsBuilder<SearchDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            //.LogTo(Console.WriteLine)
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        SeedContext = new SearchDbContext(contextOptions);
        await SeedContext.Database.EnsureCreatedAsync();

        VerifyContext = new SearchDbContext(contextOptions);
        VerifyContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }
}