using SympliSearch.ApiService.Infrastructure.Interceptors;
using SympliSearch.ApiService.Services;
using Microsoft.AspNetCore.Identity;
using SympliSearch.ApiService.Services.Search;

namespace SympliSearch.ApiService.Infrastructure;

public static class AddApplicationDependencyInjection
{
    public static void AddApplication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<IDomainEventDispatcher, MassTransitDomainEventDispatcher>();
        builder.Services.AddTransient<ICacheService, CacheService>();
        builder.Services.AddTransient<IFileService, FileService>();

        builder.Services.AddScoped<SearchEngineFactory>();
        builder.Services.AddScoped<SearchManager>();
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
}
