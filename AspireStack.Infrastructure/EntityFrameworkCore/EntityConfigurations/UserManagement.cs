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
        }
    }
}
