var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL container with persistent storage

var username = builder.AddParameter("username", "postgres", secret: true);
var password = builder.AddParameter("password", "postgres", secret: true);

var postgres = builder.AddPostgres("geoplanner", username, password).WithPgAdmin().WithDataVolume("geoplannerdbvolume").WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<PostgresDatabaseResource> db = postgres.AddDatabase("geoplannerdb", "geoplannerdb");

var migrationService = builder.AddProject<Projects.GeoPlanner_DbInitializator>("migration")
    .WithReference(db)
    .WaitFor(db)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName);

var geoplannerapi = builder.AddProject<Projects.GeoPlanner_WebApi>("geoplannerapi")
    .WithReference(db)
    .WaitFor(db)
    .WaitForCompletion(migrationService)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("angular", "../GeoPlanner.Angular")
    .WithReference(geoplannerapi)
    .WaitFor(geoplannerapi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
