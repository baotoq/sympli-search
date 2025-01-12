using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.RateLimiting;
using Ardalis.GuardClauses;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using SearchService.Application;
using SearchService.Application.Common.Exceptions;
using SearchService.Application.Common.Interfaces;
using SearchService.Application.Services;
using SearchService.Application.Services.Search;
using SearchService.Infrastructure.Data;
using SearchService.Infrastructure.Data.Interceptors;
using SearchService.Infrastructure.Dispatcher;
using SearchService.Infrastructure.Services;
using StackExchange.Redis;

namespace SearchService.Infrastructure;

public static class AddInfrastructureDependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(_ => _
            .AddFixedWindowLimiter(policyName: "default", options =>
            {
                options.PermitLimit = 4;
                options.Window = TimeSpan.FromSeconds(12);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 2;
            }));

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorizationBuilder();
        builder.AddEfCore();
        builder.AddMassTransit();

        builder.AddRedisDistributedCache("redis");
        builder.AddRedisOutputCache("redis");
#pragma warning disable EXTEXP0018
        builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018
        builder.AddRedLock();

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<SearchDbContext>());
        builder.Services.AddTransient<IDomainEventDispatcher, MassTransitDomainEventDispatcher>();
        builder.Services.AddScoped<ISearchEngineFactory, SearchEngineFactory>();
        builder.Services.AddTransient<ISearchEngine, GoogleSearchEngine>();
        builder.Services.AddTransient<ISearchEngine, BingSearchEngine>();
        builder.Services.AddTransient<GoogleSearchEngine>();
        builder.Services.AddTransient<BingSearchEngine>();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = false;
        });
    }

    private static void AddRedLock(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDistributedLockFactory>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("redis");
            Guard.Against.NullOrEmpty(connectionString, message: "Redis connection string is required.");

            return RedLockFactory.Create(new List<RedLockMultiplexer>
            {
                ConnectionMultiplexer.Connect(connectionString)
            }, sp.GetRequiredService<ILoggerFactory>());
        });
    }

    private static void AddEfCore(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, DateEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.Services.AddDbContext<SearchDbContext>((sp, options) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("searchdb");
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
            options.AddInterceptors(sp.GetServices<DateEntityInterceptor>());
            options.AddInterceptors(sp.GetServices<DispatchDomainEventsInterceptor>());
        });
        builder.EnrichNpgsqlDbContext<SearchDbContext>(s =>
        {
            s.DisableRetry = true;
        });
    }

    public static void AddMassTransit(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(s =>
        {
            s.AddConsumers(typeof(AddApplicationDependencyInjection).Assembly);
            s.UsingRabbitMq((context, cfg) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();
                var host = configuration.GetConnectionString("messaging");

                cfg.Host(host);
                cfg.ConfigureEndpoints(context);

                cfg.PrefetchCount = 1;
                cfg.AutoDelete = true;
            });

            s.AddEntityFrameworkOutbox<SearchDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });
        });
    }
}
