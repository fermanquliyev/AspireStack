using GeoPlanner.Domain.Repository;
using GeoPlanner.Infrastructure.EntityFrameworkCore;
using GeoPlanner.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPlanner.Infrastructure
{
    public static class InfrastructureModule
    {
        /// <summary>
        /// Register infrastructure module
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <param name="dbConnectionName">The name of the database connection string</param>
        public static void RegisterInfrastructureModule(this IHostApplicationBuilder builder, string dbConnectionName)
        {
            builder.AddPostgresDbContext<GeoPlannerDbContext>(dbConnectionName);
            builder.Services.AddScoped<DbContext>(b => b.GetRequiredService<GeoPlannerDbContext>());
            builder.Services.AddScoped<IAsyncQueryableExecuter, AsyncQueryableExecuter>();
            builder.Services.AddScoped<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>();
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(EfCoreRepository<,>));
        }

        private static IHostApplicationBuilder AddPostgresDbContext<TDbContext>(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<NpgsqlDbContextOptionsBuilder>? configure = null) where TDbContext : DbContext
        {
            builder.Services.AddDbContextPool<TDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString(connectionName), npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(5);
                    configure?.Invoke(npgsqlOptions);
                });
            });

            return builder;
        }
    }
}
