using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var searchdb = postgres.AddDatabase("searchdb");
var identitydb = postgres.AddDatabase("identitydb");

var storage = builder.AddAzureStorage("storage");

if (builder.Environment.IsDevelopment())
{
    storage.RunAsEmulator(s => s
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume()
        .WithHttpEndpoint(10000, 10000));
}

var cache = builder.AddRedis("redis")
    .WithRedisInsight()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithDataVolume()
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

var migrationService = builder.AddProject<Projects.SympliSearch_MigrationService>("migrationservice")
    .WithReference(searchdb).WaitFor(searchdb)
    .WithReference(identitydb).WaitFor(identitydb)
    .WithHttpHealthCheck("/health");

var searchService = builder.AddProject<Projects.SearchService_Api>("searchservice")
    .WithReference(cache)
    .WithReference(rabbitmq).WaitFor(rabbitmq)
    .WithReference(searchdb).WaitFor(searchdb)
    .WithHttpHealthCheck("/health");

var identityService = builder.AddProject<Projects.IdentityService_Api>("identityservice")
    .WithReference(cache)
    .WithReference(identitydb).WaitFor(identitydb)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
