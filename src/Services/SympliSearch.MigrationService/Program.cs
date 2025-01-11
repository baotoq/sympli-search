using MassTransit;
using MassTransit.Transports;
using SympliSearch.MigrationService;
using SympliSearch.ServiceDefaults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SympliSearch.Domain.Entities;
using SympliSearch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(DbInitializer.ActivitySourceName));

builder.AddNpgsqlDbContext<ApplicationDbContext>("db", settings =>
    {
        settings.DisableRetry = true;
    },
    options =>
    {
        options.UseNpgsql(b => b.MigrationsAssembly(typeof(Program).Assembly)).UseSnakeCaseNamingConvention();
    });

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddSingleton<DbInitializer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DbInitializer>());
builder.Services.AddHealthChecks()
    .AddCheck<DbInitializerHealthCheck>("DbInitializer");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/reset", async (ApplicationDbContext dbContext,
        DbInitializer dbInitializer,
        IHostEnvironment environment, UserManager<User> userManager, CancellationToken cancellationToken) =>
    {
        // Delete and recreate the database. This is useful for development scenarios to reset the database to its initial state.
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbInitializer.InitializeDatabaseAsync(dbContext, environment, userManager, cancellationToken);

        return Results.Ok("ok");
    });
}

app.MapDefaultEndpoints();

app.MapOpenApiEndpoints();

app.Run();

// dotnet ef migrations add Init -s src/MicroCommerce.MigrationService -p src/MicroCommerce.MigrationService --context ApplicationDbContext
