using IdentityService.Application;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure;
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

#pragma warning disable EXTEXP0018
builder.AddInfrastructure();
#pragma warning restore EXTEXP0018
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

app.Run();
