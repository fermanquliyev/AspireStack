using AspireStack.Application.AppService;
using AspireStack.Application.Security;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.UserManagement
{
    [AppServiceAuthorize]
    public class UsersAppService: IAppService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUser<Guid> currentUser;

        public UsersAppService(IUnitOfWork unitOfWork, ICurrentUser<Guid> currentUser)
        {
            this.unitOfWork = unitOfWork;
            this.currentUser = currentUser;
        }

        
        public async Task<List<User>> GetUsersAsync()
        {
            return await unitOfWork.Repository<User, Guid>().GetListAsync();
        }
    }
}
