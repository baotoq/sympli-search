using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using MassTransit;
using MassTransit.Transports;
using SympliSearch.MigrationService;
using SympliSearch.ServiceDefaults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SearchService.Domain.Entities;
using SearchService.Infrastructure;
using SearchService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(DbInitializer.ActivitySourceName));

builder.AddNpgsqlDbContext<IdentityDbContext>("identitydb", settings =>
    {
        settings.DisableRetry = true;
    },
    options =>
    {
        options.UseNpgsql(b => b.MigrationsAssembly(typeof(Program).Assembly)).UseSnakeCaseNamingConvention();
    });

builder.AddNpgsqlDbContext<SearchDbContext>("searchdb", settings =>
    {
        settings.DisableRetry = true;
    },
    options =>
    {
        options.UseNpgsql(b => b.MigrationsAssembly(typeof(Program).Assembly)).UseSnakeCaseNamingConvention();
    });

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<IdentityDbContext>();
builder.Services.AddSingleton<DbInitializer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<DbInitializer>());
builder.Services.AddHealthChecks()
    .AddCheck<DbInitializerHealthCheck>("DbInitializer");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
}

app.MapDefaultEndpoints();

app.MapOpenApiEndpoints();

app.Run();

// dotnet ef migrations add Init -s src/MicroCommerce.MigrationService -p src/MicroCommerce.MigrationService --context ApplicationDbContext
