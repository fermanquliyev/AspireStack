using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using AspireStack.Domain.Cache;
using AspireStack.Infrastructure.Cache;
using AspireStack.Infrastructure.EntityFrameworkCore;
using AspireStack.Infrastructure.Jwt;
using AspireStack.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using AspireStack.Domain.Localization;

namespace AspireStack.Infrastructure
{
    public static class InfrastructureModule
    {
        /// <summary>
        /// Register infrastructure module with AspireStackDbContext
        /// </summary>
        /// <param name="builder">The application builder</param>
        /// <param name="dbConnectionName">The name of the database connection string</param>
        public static void RegisterInfrastructureModule(this IHostApplicationBuilder builder, string dbConnectionName)
        {
            builder.AddPostgresDbContext<AspireStackDbContext>(dbConnectionName);
            AddInfrastructureServices(builder);
            builder.Services.AddScoped<DbContext, AspireStackDbContext>();
        }

        /// <summary>
        /// Register infrastructure module with only services
        /// </summary>
        /// <param name="builder"></param>
        public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAsyncQueryableExecuter, AsyncQueryableExecuter>();
            builder.Services.AddScoped<IAsyncQueryableProvider, EfCoreAsyncQueryableProvider>();
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(EfCoreRepository<,>));
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(EfUnitOfWork));
            builder.Services.AddScoped(typeof(IUserTokenHandler), typeof(JwtTokenHandler));
            builder.Services.AddScoped<ICurrentUser<Guid>, CurrentUser<Guid>>();
            builder.Services.AddScoped<ICurrentUser, CurrentUser>();
            builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
            builder.Services.AddScoped(typeof(IUserPasswordHasher<>), typeof(DefaultPasswordHasher<>));
            builder.Services.AddScoped<ICacheClient, CacheClient>();
            builder.Services.AddSingleton<ILocalizationProvider, LocalizationProvider>();
        }

        private static IHostApplicationBuilder AddPostgresDbContext<TDbContext>(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<NpgsqlDbContextOptionsBuilder>? configure = null) where TDbContext : DbContext
        {
            builder.Services.AddDbContext<TDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString(connectionName), npgsqlOptions =>
                {
                    configure?.Invoke(npgsqlOptions);
                });
            });

            return builder;
        }
    }
}
