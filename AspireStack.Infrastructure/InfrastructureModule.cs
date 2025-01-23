using AspireStack.Domain.Repository;
using AspireStack.Infrastructure.EntityFrameworkCore;
using AspireStack.Infrastructure.Repository;
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

namespace AspireStack.Infrastructure
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
            builder.AddPostgresDbContext<AspireStackDbContext>(dbConnectionName);
            builder.Services.AddScoped<DbContext>(b => b.GetRequiredService<AspireStackDbContext>());
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
