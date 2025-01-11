using System.Diagnostics;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;
using SearchService.Domain.Entities;
using SearchService.Infrastructure;

namespace SympliSearch.MigrationService;

public class DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Initializing database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            await InitializeDatabaseAsync(context, environment, userManager, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
    }

    public async Task InitializeDatabaseAsync(ApplicationDbContext context, IHostEnvironment environment, UserManager<User> userManager,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(context.Database.MigrateAsync, cancellationToken);

        await SeedDataAsync(context, environment, userManager, cancellationToken);

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
    }

    private static async Task SeedDataAsync(ApplicationDbContext context, IHostEnvironment environment, UserManager<User> userManager, CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
