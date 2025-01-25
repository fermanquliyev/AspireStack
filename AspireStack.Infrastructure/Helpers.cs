using AspireStack.Domain.Entities;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Infrastructure.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspireStack.Infrastructure
{
    public static class Helpers
    {
        public static bool AddAdminUser(this AspireStackDbContext dbContext, string username, string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            var dbUser = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (dbUser != null)
            {
                return false;
            }
            dbUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "User",
                Email = $"{username}@aspirestack.com",
                EmailVerified = true,
                Username = username
            };
            dbContext.Users.Add(dbUser);
            var passwordHash = passwordHasher.HashPassword(dbUser, password);
            dbUser.PasswordHashed = passwordHash;
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
