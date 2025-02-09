using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.AppService.DTOs
{
    public interface IPagedResultRequest
    {
        int Page { get; set; }
        int PageSize { get; set; }
    }
}
