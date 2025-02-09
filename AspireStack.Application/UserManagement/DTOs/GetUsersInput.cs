using AspireStack.Application.AppService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.UserManagement.DTOs
{
    public class GetUsersInput: IPagedResultRequest
    {
        public string Filter { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
