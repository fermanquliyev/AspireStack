using AspireStack.Domain.Entities;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Shared.UserManagement;
using AspireStack.Infrastructure.EntityFrameworkCore;
using AspireStack.Infrastructure.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspireStack.Infrastructure
{
    public static class Helpers
    {
        public static bool AddAdminUser(this AspireStackDbContext dbContext, string username, string password)
        {
            var passwordHasher = new DefaultPasswordHasher<User>(new PasswordHasher<User>());
            var adminRole = dbContext.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Name = "Admin",
                    Description = "Admin role"
                    // Permissions = PermissionNames.PermissionStrings.Select(x => x.Value).ToArray()
                };
                dbContext.Roles.Add(adminRole);
                dbContext.SaveChanges();
                var roleClaims = PermissionNames.PermissionStrings.Select(x => new RoleClaim
                {
                    RoleId = adminRole.Id,
                    ClaimType = CustomClaimTypes.Permission,
                    ClaimValue = x
                });
                dbContext.RoleClaims.AddRange(roleClaims);
            }
            var adminUser = dbContext.Users.FirstOrDefault(u => u.UserName == username);
            if (adminUser != null)
            {
                return false;
            }
            adminUser = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = $"{username}@gmail.com",
                NormalizedEmail = $"{username}@gmail.com".ToUpper(),
                NormalizedUserName = username.ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailVerified = true,
                UserName = username
            };
            dbContext.Users.Add(adminUser);
            var userRole = new UserRole
            {
                RoleId = adminRole.Id,
                UserId = adminUser.Id
            };
            dbContext.UserRoles.Add(userRole);
            var passwordHash = passwordHasher.HashPassword(adminUser, password);
            adminUser.PasswordHash = passwordHash;
            dbContext.SaveChanges();
            return true;
        }

        public static EntityTypeBuilder<TEntity> ConfigureAuditProperties<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder) where TEntity : class, IAuditedEntity
        {
            entityTypeBuilder.Property(e => e.IsDeleted).IsRequired();
            entityTypeBuilder.HasQueryFilter(e => !e.IsDeleted);
            return entityTypeBuilder;
        }

        public static EntityTypeBuilder<TEntity> ConfigureSoftDeleteProperties<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder) where TEntity : class, ISoftDelete
        {
            entityTypeBuilder.Property(e => e.IsDeleted).IsRequired();
            entityTypeBuilder.HasQueryFilter(e => !e.IsDeleted);
            return entityTypeBuilder;
        }
    }
}
