using System.Diagnostics;
using System.Reflection;
using Ardalis.GuardClauses;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using SympliSearch.Infrastructure.Behaviour;
using SympliSearch.Infrastructure.Exceptions;
using SympliSearch.Infrastructure.Interceptors;
using User = SympliSearch.Domain.Entities.User;

namespace SympliSearch.Infrastructure;

public static class AddInfrastructureDependencyInjection
{
    public static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                context.ProblemDetails.Extensions.TryAdd("traceId", Activity.Current?.Id);
            };
        });
        builder.Services.AddExceptionHandler<InvalidValidationExceptionHandler>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetCallingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });
        builder.Services.AddValidatorsFromAssembly(Assembly.GetCallingAssembly());

        builder.AddEfCore();
        builder.AddMassTransit(Assembly.GetCallingAssembly());

        builder.AddRedisDistributedCache("redis");
        builder.AddRedisOutputCache("redis");
        builder.AddRedLock();

        builder.AddAuthorization();
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

    private static void AddAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddAuthorizationBuilder();
    }

    private static void AddEfCore(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISaveChangesInterceptor, DateEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("db");
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
            options.AddInterceptors(sp.GetServices<DateEntityInterceptor>());
            options.AddInterceptors(sp.GetServices<DispatchDomainEventsInterceptor>());
        });
        builder.EnrichNpgsqlDbContext<ApplicationDbContext>(s =>
        {
            s.DisableRetry = true;
        });
    }

    public static void AddMassTransit(this IHostApplicationBuilder builder, Assembly assembly)
    {
        builder.Services.AddMassTransit(s =>
        {
            s.AddConsumers(assembly);
            s.UsingRabbitMq((context, cfg) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();
                var host = configuration.GetConnectionString("messaging");

                cfg.Host(host);
                cfg.ConfigureEndpoints(context);

                cfg.PrefetchCount = 1;
                cfg.AutoDelete = true;
            });

            s.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });
        });
    }
}
