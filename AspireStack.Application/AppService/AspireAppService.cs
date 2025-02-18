using AspireStack.Domain.Cache;
using AspireStack.Domain.Localization;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.AppService
{
    public abstract class AspireAppService: IAspireAppService
    {
        public required ICurrentUser CurrentUser { get; set; }
        public required IUnitOfWork UnitOfWork { get; set; }
        public required IAsyncQueryableExecuter AsyncExecuter { get; set; }
        public required ICacheClient CacheClient { get; set; }
        public required ILocalizationProvider LocalizationProvider { get; set; }

        /// <summary>
        /// Initialize the required services,repos from unit of work. 
        /// Do not call this method in constructor. 
        /// It will called automatically durig http call.
        /// </summary>
        public virtual void Init()
        {
            // Initialize the required services,repos from unit of work.
        }

        /// <summary>
        /// Get the localized string from the resource file.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string L(string name)
        {
            var key = LocalizationProvider.GetResource(name);
            return key;
        }

        /// <summary>
        /// Get the localized string from the resource file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected string L(string name,object arg)
        {
            var key = LocalizationProvider.GetResource(name);
            return string.Format(key, arg);
        }

        /// <summary>
        /// Get the localized string from the resource file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        protected string L(string name, object arg1, object arg2)
        {
            var key = LocalizationProvider.GetResource(name);
            return string.Format(key, arg1, arg2);
        }

        /// <summary>
        /// Get the localized string from the resource file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected string L(string name, params object[] args)
        {
            var key = LocalizationProvider.GetResource(name);
            return string.Format(key, args);
        }
    }
}
