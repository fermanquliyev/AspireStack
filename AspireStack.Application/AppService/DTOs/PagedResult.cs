using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.AppService.DTOs
{
    public class PagedResult<T> where T : class
    {
        public PagedResult()
        {
            
        }

        public PagedResult(IList<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
        public IList<T> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
