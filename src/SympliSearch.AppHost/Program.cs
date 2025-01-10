var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.SympliSearch_ApiService>("apiservice");

builder.AddProject<Projects.SympliSearch_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
