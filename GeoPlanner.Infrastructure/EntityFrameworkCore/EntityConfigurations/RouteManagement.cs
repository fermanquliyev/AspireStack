using GeoPlanner.Domain.Entities.LocationManagement;
using GeoPlanner.Domain.Entities.RouteManagement;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPlanner.Infrastructure.EntityFrameworkCore.EntityConfigurations
{
    public static class RouteManagement
    {
        public static void ConfigureRouteManagement(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("Routes", "RouteManagement");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Description)
                    .HasMaxLength(500);
            });
        }
    }
}
