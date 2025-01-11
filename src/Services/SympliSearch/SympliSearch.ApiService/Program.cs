using SympliSearch.Application;
using SympliSearch.Domain.Entities;
using SympliSearch.Infrastructure;
using SympliSearch.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(options => options.AddDefaultPolicy(
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    )
);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.AddInfrastructure();
builder.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
}

app.UseExceptionHandler();
app.UseCors();

app.UseRequestTimeouts();

app.UseOutputCache();
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapOpenApiEndpoints();

app.MapIdentityApi<User>().WithTags("Identity");
app.MapEndpoints();

app.Run();
