using AspireStack.Domain.Entities;
using AspireStack.Domain.Entities.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.ValueGeneration;

namespace AspireStack.Infrastructure.EntityFrameworkCore.EntityConfigurations
{
    public static class UserManagement
    {
        const int maxKeyLength = 127;
        public static void ConfigureUserManagement(this ModelBuilder builder)
        {
            builder.Entity<User>(b =>
            {
                // Primary key
                b.HasKey(u => u.Id);

                b.Property(e => e.Id).HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

                // Indexes for "normalized" username and email, to allow efficient lookups
                b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");

                // Maps to the AspNetUsers table
                b.ToTable("Users", "UserManagement");

                // A concurrency token for use with the optimistic concurrency checking
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                // Limit the size of columns to use efficient database types
                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.Property(u => u.Email).HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);

                // The relationships between User and other entity types
                // Note that these relationships are configured with no navigation properties

                // Each User can have many UserClaims
                b.HasMany<IdentityUserClaim<Guid>>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

                // Each User can have many UserLogins
                b.HasMany<IdentityUserLogin<Guid>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

                // Each User can have many UserTokens
                b.HasMany<IdentityUserToken<Guid>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany<UserRole>(u=>u.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired();

                b.ConfigureAuditProperties();
            });

            builder.Entity<IdentityUserClaim<Guid>>(b =>
            {
                // Primary key
                b.HasKey(uc => uc.Id);

                // Maps to the AspNetUserClaims table
                b.ToTable("UserClaims", "UserManagement");
            });

            builder.Entity<IdentityUserLogin<Guid>>(b =>
            {
                // Composite primary key consisting of the LoginProvider and the key to use
                // with that provider
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

                // Limit the size of the composite key columns due to common DB restrictions
                b.Property(l => l.LoginProvider).HasMaxLength(128);
                b.Property(l => l.ProviderKey).HasMaxLength(128);

                // Maps to the AspNetUserLogins table
                b.ToTable("UserLogins", "UserManagement");
            });

            builder.Entity<IdentityUserToken<Guid>>(b =>
            {
                // Composite primary key consisting of the UserId, LoginProvider and Name
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                // Limit the size of the composite key columns due to common DB restrictions
                b.Property(t => t.LoginProvider).HasMaxLength(maxKeyLength);
                b.Property(t => t.Name).HasMaxLength(maxKeyLength);

                // Maps to the AspNetUserTokens table
                b.ToTable("UserTokens", "UserManagement");
            });

            builder.Entity<Role>(b =>
            {
                // Primary key
                b.HasKey(r => r.Id);

                b.Property(e => e.Id).HasValueGenerator<NpgsqlSequentialGuidValueGenerator>();

                // Index for "normalized" role name to allow efficient lookups
                b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

                // Maps to the AspNetRoles table
                b.ToTable("Roles", "UserManagement");

                // A concurrency token for use with the optimistic concurrency checking
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                // Limit the size of columns to use efficient database types
                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);

                // The relationships between Role and other entity types
                // Note that these relationships are configured with no navigation properties

                // Each Role can have many entries in the UserRole join table
                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany<RoleClaim>(x=>x.RoleClaims).WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();

                b.ConfigureAuditProperties();
            });

            builder.Entity<RoleClaim>(b =>
            {
                // Primary key
                b.HasKey(rc => rc.Id);

                // Maps to the AspNetRoleClaims table
                b.ToTable("RoleClaims", "UserManagement");
            });

            builder.Entity<UserRole>(b =>
            {
                // Primary key
                b.HasKey(r => new { r.UserId, r.RoleId });

                b.HasOne<Role>(ur => ur.Role).WithMany().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasOne<User>(ur => ur.User).WithMany().HasForeignKey(ur => ur.UserId).IsRequired();

                // Maps to the AspNetUserRoles table
                b.ToTable("UserRoles", "UserManagement");
            });
        }
    }
}
