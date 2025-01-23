using GeoPlanner.Domain.Entities;
using GeoPlanner.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Data;
using System.Linq.Dynamic.Core;

namespace GeoPlanner.Infrastructure.Repository
{
    public class EfCoreRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : Entity<TKey>
    {
        private readonly DbContext dbContext;
        private readonly IAsyncQueryableExecuter asyncQueryableExecuter;

        public EfCoreRepository(DbContext dbContext, IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            this.dbContext = dbContext;
            this.asyncQueryableExecuter = asyncQueryableExecuter;
        }

        public IAsyncQueryableExecuter AsyncExecuter => asyncQueryableExecuter;

        public async Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entities = await dbContext.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
            dbContext.Set<TEntity>().RemoveRange(entities);
            if (autoSave)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            dbContext.Set<TEntity>().Remove(entity);
            if (autoSave)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteDirectAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await dbContext.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
            dbContext.Set<TEntity>().RemoveRange(entities);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteManyAsync([NotNull] IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            dbContext.Set<TEntity>().RemoveRange(entities);
            if (autoSave)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<TEntity?> FindAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<TEntity> GetAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<TEntity>().SingleAsync(predicate, cancellationToken);
        }

        public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<TEntity>().LongCountAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetListAsync([NotNull] Expression<Func<TEntity, bool>> predicate, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public async Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            var set = dbContext.Set<TEntity>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(sorting))
            {
                set = set.OrderBy(sorting);
            }

            return await set
                .Skip(skipCount)
                .Take(maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public async Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            return await Task.FromResult(dbContext.Set<TEntity>().AsQueryable());
        }

        public async Task<TEntity> InsertAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
            if (autoSave)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            return entity;
        }

        public async Task InsertManyAsync([NotNull] IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
            if (autoSave)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<TEntity> UpdateAsync([NotNull] TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            dbContext.Set<TEntity>().Update(entity);
            if (autoSave)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            return entity;
        }

        public async Task UpdateManyAsync([NotNull] IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            dbContext.Set<TEntity>().UpdateRange(entities);
            if (autoSave)
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public IQueryable<TEntity> WithDetails()
        {
            return dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = dbContext.Set<TEntity>().AsQueryable();
            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }
            return query;
        }

        public async Task<IQueryable<TEntity>> WithDetailsAsync()
        {
            return await Task.FromResult(dbContext.Set<TEntity>().AsQueryable());
        }

        public async Task<IQueryable<TEntity>> WithDetailsAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = dbContext.Set<TEntity>().AsQueryable();
            foreach (var propertySelector in propertySelectors)
            {
                query = query.Include(propertySelector);
            }
            return await Task.FromResult(query);
        }
    }
}
