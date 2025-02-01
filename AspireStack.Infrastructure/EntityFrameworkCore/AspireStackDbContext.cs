using AspireStack.Domain.Entities;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Services;
using AspireStack.Infrastructure.EntityFrameworkCore.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AspireStack.Infrastructure.EntityFrameworkCore
{
    public class AspireStackDbContext(DbContextOptions<AspireStackDbContext> options, ICurrentUser<Guid> currentUser) : DbContext(options)
    {
        private readonly ICurrentUser<Guid> _currentUser = currentUser;
        private bool ChangesTracked { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureUserManagement();

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AuditEntityChanges();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AuditEntityChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void AuditEntityChanges()
        {
            if (ChangeTracker.HasChanges() && !ChangesTracked)
            {
                var validatedEntries = ChangeTracker.Entries()
                    .Where(e => typeof(IValidatedEntity).IsAssignableFrom(e.Entity.GetType()) &&
                               (e.State == EntityState.Added || e.State == EntityState.Modified));
                var added = ChangeTracker.Entries()
                    .Where(e => typeof(IAuditedEntity).IsAssignableFrom(e.Entity.GetType()) &&
                                e.State == EntityState.Added);
                var modified = ChangeTracker.Entries()
                    .Where(e => typeof(IAuditedEntity).IsAssignableFrom(e.Entity.GetType()) &&
                                e.State == EntityState.Modified);
                var deleted = ChangeTracker.Entries()
                    .Where(e => (typeof(IAuditedEntity).IsAssignableFrom(e.Entity.GetType()) || typeof(ISoftDelete).IsAssignableFrom(e.Entity.GetType())) &&
                                e.State == EntityState.Deleted);

                foreach (var entity in validatedEntries)
                {
                    ((IValidatedEntity)entity.Entity).Validate();
                }

                foreach (var entity in added)
                {
                    entity.Property("CreationTime").CurrentValue = DateTime.UtcNow;
                    entity.Property("CreatorId").CurrentValue = _currentUser.Id;
                }

                foreach (var entity in modified)
                {
                    entity.Property("LastModificationTime").CurrentValue = DateTime.UtcNow;
                    entity.Property("LastModifierId").CurrentValue = _currentUser.Id;
                }

                foreach (var entity in deleted)
                {
                    entity.State = EntityState.Modified;
                    entity.Property("IsDeleted").CurrentValue = true;
                    entity.Property("DeletionTime").CurrentValue = DateTime.UtcNow;
                }
                ChangesTracked = true;
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}
