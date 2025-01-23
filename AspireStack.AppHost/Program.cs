var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL container with persistent storage

var username = builder.AddParameter("username", "postgres", secret: true);
var password = builder.AddParameter("password", "postgres", secret: true);

var postgres = builder.AddPostgres("AspireStackPostgresServer", username, password).WithPgAdmin().WithDataVolume("AspireStackDbVolume").WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<PostgresDatabaseResource> db = postgres.AddDatabase("AspireStackDb", "AspireStackDb");

var migrationService = builder.AddProject<Projects.AspireStack_DbInitializator>("migration")
    .WithReference(db)
    .WaitFor(db)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var AspireStackapi = builder.AddProject<Projects.AspireStack_WebApi>("AspireStackApi")
    .WithReference(db)
    .WaitFor(db)
    .WaitForCompletion(migrationService)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("angular", "../AspireStack.Angular")
    .WithReference(AspireStackapi)
    .WaitFor(AspireStackapi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
