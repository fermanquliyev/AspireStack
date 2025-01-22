using GeoPlanner.Domain.Entities.LocationManagement;
using GeoPlanner.Domain.Entities.RouteManagement;
using GeoPlanner.Domain.Entities.UserManagement;
using GeoPlanner.Infrastructure.EntityFrameworkCore.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPlanner.Infrastructure.EntityFrameworkCore
{
    public class GeoPlannerDbContext(DbContextOptions<GeoPlannerDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureUserManagement();
            modelBuilder.ConfigureLocationManagement();
            modelBuilder.ConfigureRouteManagement();

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Route> Routes { get; set; }
    }
}
