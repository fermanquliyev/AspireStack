using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Infrastructure.EntityFrameworkCore.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Infrastructure.EntityFrameworkCore
{
    public class AspireStackDbContext(DbContextOptions<AspireStackDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureUserManagement();

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }
    }
}
