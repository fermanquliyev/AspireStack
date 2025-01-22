using GeoPlanner.Domain.Entities.LocationManagement;
using GeoPlanner.Domain.Entities.UserManagement;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPlanner.Infrastructure.EntityFrameworkCore.EntityConfigurations
{
    public static class LocationManagement
    {
        public static void ConfigureLocationManagement(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Locations", "LocationManagement");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(80);
                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.State)
                    .HasMaxLength(50);
                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.PostalCode)
                    .HasMaxLength(50);
                entity.Property(e => e.Latitude)
                    .IsRequired();
                entity.Property(e => e.Longitude)
                    .IsRequired();
                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e=> e.UserId);
                entity.HasIndex(e => e.RouteId);
            });
        }
    }
}
