using AspireStack.Domain.Entities;
using AspireStack.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Infrastructure.Repository
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly IAsyncQueryableExecuter asyncQueryableExecuter;
        private readonly Dictionary<Type, object> _repositories = new();

        public IAsyncQueryableExecuter AsyncQueryableExecuter => asyncQueryableExecuter;

        public EfUnitOfWork(DbContext context, IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            _context = context;
            this.asyncQueryableExecuter = asyncQueryableExecuter;
        }

        public IRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : Entity<TKey>
        {
            if (!_repositories.ContainsKey(typeof(TEntity)))
            {
                var repositoryInstance = new EfCoreRepository<TEntity,TKey>(_context);
                _repositories.Add(typeof(TEntity), repositoryInstance);
            }

            return (IRepository<TEntity,TKey>)_repositories[typeof(TEntity)];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }

}
