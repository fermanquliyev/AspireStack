using AspireStack.Domain.Cache;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.AppService
{
    public abstract class AspireAppService
    {
        public required ICurrentUser CurrentUser { get; set; }
        public required IUnitOfWork UnitOfWork { get; set; }
        public required IAsyncQueryableExecuter AsyncExecuter { get; set; }

        public required ICacheClient CacheClient { get; set; }

        /// <summary>
        /// Initialize the required services,repos from unit of work. 
        /// Do not call this method in constructor. 
        /// It will called automatically durig http call.
        /// </summary>
        public virtual void Init()
        {
            // Initialize the required services,repos from unit of work.
        }
    }
}
