using AspireStack.Application.AppService;
using AspireStack.Application.AppService.DTOs;
using AspireStack.Application.UserManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.UserManagement
{
    public interface IUserAppService: IAspireAppService
    {
        Task<Guid> CreateUserAsync(CreateEditUserDto input);
        Task DeleteUserAsync(Guid id);
        Task<CreateEditUserDto> GetUserByIdAsync(Guid id);
        Task<PagedResult<UserDto>> GetUsersAsync(GetUsersInput usersInput);
        Task ResetPasswordAsync(Guid id);
        Task UpdateCurrentUserAsync(UpdateCurrentUserDto input);
        Task UpdateUserAsync(CreateEditUserDto input);
    }
}
