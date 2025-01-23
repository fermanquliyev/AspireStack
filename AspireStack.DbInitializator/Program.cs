using AspireStack.DbInitializator;
using AspireStack.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ApiDbInitializer>();

builder.AddServiceDefaults();

var envName = builder.Configuration["ASPNETCORE_ENVIRONMENT"];
builder.Environment.EnvironmentName = envName;

builder.Services.AddDbContextPool<AspireStackDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AspireStackDb");
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(5);
        npgsqlOptions.MigrationsAssembly("AspireStack.DbInitializator");
        // Workaround for https://github.com/dotnet/aspire/issues/1023
        npgsqlOptions.ExecutionStrategy(c => new RetryingSqlServerRetryingExecutionStrategy(c));
    });
});
builder.EnrichNpgsqlDbContext<AspireStackDbContext>(settings =>
    // Disable Aspire default retries as we're using a custom execution strategy
    settings.DisableRetry = true);

var app = builder.Build();



app.Run();
