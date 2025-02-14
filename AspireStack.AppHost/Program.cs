var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL container with persistent storage

var username = builder.AddParameter("username", "postgres", secret: true);
var password = builder.AddParameter("password", "postgres", secret: true);

var postgres = builder.AddPostgres("AspireStackPostgresServer", username, password).WithPgAdmin().WithDataVolume("AspireStackDbVolume").WithLifetime(ContainerLifetime.Persistent);
    //.PublishAsAzurePostgresFlexibleServer();

IResourceBuilder<PostgresDatabaseResource> db = postgres.AddDatabase("AspireStackDb", "AspireStackDb");

var cache = builder.AddRedis("cache")
    .WithRedisInsight()
    .WithDataVolume();

var migrationService = builder.AddProject<Projects.AspireStack_DbInitializator>("migration")
    .WithReference(db)
    .WaitFor(db)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var AspireStackApi = builder.AddProject<Projects.AspireStack_WebApi>("AspireStackApi")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(cache)
    .WaitFor(cache)
    .WaitForCompletion(migrationService)
    .WithExternalHttpEndpoints();

var serviceGenerator = builder.AddProject<Projects.AspireStack_Angular_ServiceGenerator>("aspirestack-angular-servicegenerator")
    .WithReference(AspireStackApi)
    .WaitFor(AspireStackApi);

builder.AddNpmApp("aspirestackui", "../AspireStack.Angular")
    .WithReference(AspireStackApi)
    .WaitFor(AspireStackApi)
    //.WaitFor(serviceGenerator)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
