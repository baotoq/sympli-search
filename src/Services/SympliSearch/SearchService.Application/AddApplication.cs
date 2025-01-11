using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SearchService.Application.Dispatcher;
using SympliSearch.Application.Services;
using SympliSearch.Application.Services.Search;
using SearchService.Infrastructure.Dispatcher;

namespace SympliSearch.Application;

public static class AddApplicationDependencyInjection
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<IDomainEventDispatcher, MassTransitDomainEventDispatcher>();
        builder.Services.AddTransient<ICacheService, CacheService>();

        builder.Services.AddScoped<SearchEngineFactory>();
        builder.Services.AddScoped<SearchManager>();
        builder.Services.AddTransient<GoogleSearchEngine>();
        builder.Services.AddTransient<BingSearchEngine>();

        builder.Services.AddEndpoints();

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
}
