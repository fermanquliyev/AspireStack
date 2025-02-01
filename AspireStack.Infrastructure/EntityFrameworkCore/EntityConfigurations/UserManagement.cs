using AspireStack.Domain.Entities;
using AspireStack.Domain.Entities.UserManagement;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace AspireStack.Infrastructure.EntityFrameworkCore.EntityConfigurations
{
    public static class UserManagement
    {
        public static void ConfigureUserManagement(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "UserManagement");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.HasIndex(e => e.Username)
                    .IsUnique();
                entity.Property(e => e.PasswordHashed)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.ConfigureAuditProperties();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles", "UserManagement");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Description)
                    .HasMaxLength(100);
                entity.Property(e => e.Permissions)
                    .IsRequired()
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
                entity.ConfigureAuditProperties();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles", "UserManagement");
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.Property(e => e.UserId)
                    .IsRequired();
                entity.Property(e => e.RoleId)
                    .IsRequired();
            });
        }
    }
}
