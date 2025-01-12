using System.Diagnostics;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using OpenTelemetry.Trace;
using SearchService.Domain.Entities;
using SearchService.Infrastructure;
using SearchService.Infrastructure.Data;

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
            var context = scope.ServiceProvider.GetRequiredService<SearchDbContext>();
            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var sw = Stopwatch.StartNew();

            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(context.Database.MigrateAsync, cancellationToken);

            strategy = identityContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(identityContext.Database.MigrateAsync, cancellationToken);

            if (!identityContext.Users.Any())
            {
                var result = await userManager.CreateAsync(new User
                {
                    UserName = "admin@sympli.com",
                    Email = "admin@sympli.com",
                    EmailConfirmed = true
                }, "P@ssw0rd");
            }

            logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }
    }
}
