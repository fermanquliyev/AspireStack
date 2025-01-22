var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL container with persistent storage
//var postgres = builder.AddPostgres("geoplannerdb")
//    .WithEnvironment((context) =>
//    {
//        context.EnvironmentVariables["POSTGRES_DB"] = "geoplannerdb";
//        context.EnvironmentVariables["POSTGRES_USER"] = "postgres";
//        context.EnvironmentVariables["POSTGRES_PASSWORD"] = "postgres";
//    })
//    .WithPgAdmin() // Optional admin UI
//    .WithDataVolume(name: "geoplannerdbvolume"); // Persist data between container restarts

var username = builder.AddParameter("username", "postgres", secret: true);
var password = builder.AddParameter("password", "postgres", secret: true);

var postgres = builder.AddPostgres("geoplanner", username, password).WithPgAdmin().WithDataVolume("geoplannerdbvolume").WithLifetime(ContainerLifetime.Persistent);

IResourceBuilder<PostgresDatabaseResource> db = postgres.AddDatabase("geoplannerdb", "geoplannerdb");

var geoplannerapi = builder.AddProject<Projects.GeoPlanner_WebApi>("geoplannerapi")
    .WithReference(db)
    .WaitFor(db)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("angular", "../GeoPlanner.Angular")
    .WithReference(geoplannerapi)
    .WaitFor(geoplannerapi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
