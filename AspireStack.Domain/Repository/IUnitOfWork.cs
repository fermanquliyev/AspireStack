using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspireStack.Domain.Entities;

namespace AspireStack.Domain.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : Entity<TKey>;
        Task<int> SaveChangesAsync();
        int SaveChanges();
        IAsyncQueryableExecuter AsyncQueryableExecuter { get; }
    }
}
