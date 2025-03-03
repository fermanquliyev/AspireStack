using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspireStack.Infrastructure.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AspireStack.Infrastructure.Jwt;
using Microsoft.AspNetCore.Http;

namespace AspireStack.DbInitializator
{
    /// <summary>
    /// Design time factory for creating the DbContext during Add-Migration command.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AspireStackDbContext>
    {

        public AspireStackDbContext CreateDbContext(string[] args)
        {
            // Configure DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<AspireStackDbContext>();
            optionsBuilder.UseNpgsql(string.Empty, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(5);
                npgsqlOptions.MigrationsAssembly("AspireStack.DbInitializator");
            });

            // Create and return the DbContext
            return new AspireStackDbContext(optionsBuilder.Options, new CurrentUser(new HttpContextAccessor()));
        }
    }
}
