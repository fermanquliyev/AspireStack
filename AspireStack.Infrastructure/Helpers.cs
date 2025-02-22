using AspireStack.Domain.Entities;
using AspireStack.Domain.Entities.UserManagement;
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
                    Description = "Admin role",
                    Permissions = PermissionNames.PermissionStrings.Select(x => x.Value).ToArray()
                };
                dbContext.Roles.Add(adminRole);
                dbContext.SaveChanges();
            }
            var adminUser = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (adminUser != null)
            {
                return false;
            }
            adminUser = new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = $"{username}@gmail.com",
                EmailVerified = true,
                Username = username
            };
            dbContext.Users.Add(adminUser);
            adminUser.AddRole(adminRole);
            var passwordHash = passwordHasher.HashPassword(adminUser, password);
            adminUser.PasswordHashed = passwordHash;
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
