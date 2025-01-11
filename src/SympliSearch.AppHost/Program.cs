using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var db = postgres.AddDatabase("db");

var storage = builder.AddAzureStorage("storage");

if (builder.Environment.IsDevelopment())
{
    storage.RunAsEmulator(s => s
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume()
        .WithHttpEndpoint(10000, 10000));
}

var blobs = storage.AddBlobs("blobs");

var cache = builder.AddRedis("redis")
    .WithRedisInsight()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithDataVolume()
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

var migrationService = builder.AddProject<Projects.SympliSearch_MigrationService>("migrationservice")
    .WithReference(db).WaitFor(db)
    .WithReference(rabbitmq).WaitFor(rabbitmq)
    .WithReference(blobs)
    .WithHttpHealthCheck("/health");

var apiService = builder.AddProject<Projects.SympliSearch_ApiService>("apiservice")
    .WithReference(cache)
    .WithReference(rabbitmq).WaitFor(rabbitmq)
    .WithReference(db).WaitFor(db)
    .WithReference(blobs)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.SympliSearch_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
