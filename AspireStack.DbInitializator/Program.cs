using AspireStack.DbInitializator;
using AspireStack.Infrastructure.EntityFrameworkCore;
using AspireStack.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ApiDbInitializer>();

builder.AddServiceDefaults();

var envName = builder.Configuration["ASPNETCORE_ENVIRONMENT"];
builder.Environment.EnvironmentName = envName;

builder.Services.AddDbContext<AspireStackDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AspireStackDb");

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(5);
        npgsqlOptions.MigrationsAssembly("AspireStack.DbInitializator");
    });
});

builder.Services.AddScoped<DbContext, AspireStackDbContext>();
builder.AddInfrastructureServices();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.Run();
