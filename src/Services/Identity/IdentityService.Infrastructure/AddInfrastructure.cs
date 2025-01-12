using System.Threading.RateLimiting;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityService.Infrastructure;

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

        builder.AddEfCore();

        builder.AddRedisDistributedCache("redis");
        builder.AddRedisOutputCache("redis");
#pragma warning disable EXTEXP0018
        builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018
        builder.AddAuthorization();

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());

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

    private static void AddAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<IdentityDbContext>();

        builder.Services.AddAuthorizationBuilder();
    }

    private static void AddEfCore(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("identitydb");
            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });
        builder.EnrichNpgsqlDbContext<IdentityDbContext>(s =>
        {
            s.DisableRetry = true;
        });
    }
}
