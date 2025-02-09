using AspireStack.Application.AppService;
using AspireStack.Application.Security;
using AspireStack.Application.RoleManagement.DTOs;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using AspireStack.Application.AppService.DTOs;
using System.Data;

namespace AspireStack.Application.UserManagement
{
    [AppServiceAuthorize]
    public class RoleAppService : AspireAppService
    {
        private IRepository<Role, Guid> roleRepository = default!;
        private IRepository<UserRole, string> userRoleRepository = default!;

        public override void Init()
        {
            this.roleRepository = UnitOfWork.Repository<Role, Guid>();
            this.userRoleRepository = UnitOfWork.Repository<UserRole, string>();
        }

        [AppServiceAuthorize(PermissionNames.Role_View)]
        public async Task<RoleDto> GetRoleAsync(Guid id)
        {
            var role = await AsyncExecuter.FirstOrDefaultAsync(roleRepository.GetQueryableAsNoTracking().Where(r => r.Id == id));
            if (role == null)
            {
                throw new ValidationException("Role not found");
            }
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.Permissions,
                CreationTime = role.CreationTime,
                LastModificationTime = role.LastModificationTime
            };
        }

        [AppServiceAuthorize(PermissionNames.Role_View)]
        public async Task<List<RoleDto>> GetAllRoles()
        {
            var query = roleRepository.GetQueryableAsNoTracking()
                .OrderBy(x => x.Name)
                .Select(role => new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    Permissions = role.Permissions
                });
            var roles = await AsyncExecuter.ToListAsync(query);
            return roles;
        }

        [AppServiceAuthorize(PermissionNames.Role_View)]
        public async Task<PagedResult<RoleDto>> GetAllRolesPagedAsync(PagedResultRequest pagedResultRequest)
        {
            var query = roleRepository.GetQueryableAsNoTracking()
                .OrderByDescending(x => x.CreationTime)
                .Skip((pagedResultRequest.Page - 1) * pagedResultRequest.PageSize)
                .Take(pagedResultRequest.PageSize)
                .Select(role => new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    CreationTime = role.CreationTime,
                    LastModificationTime = role.LastModificationTime
                });
            var roles = await AsyncExecuter.ToListAsync(query);
            var totalCount = await roleRepository.GetCountAsync();
            return new PagedResult<RoleDto>(roles, totalCount);
        }

        [AppServiceAuthorize(PermissionNames.Role_Create)]
        public async Task<RoleDto> CreateRoleAsync(RoleDto input)
        {
            var role = new Role
            {
                Name = input.Name,
                Description = input.Description,
                Permissions = input.Permissions,
            };

            await roleRepository.InsertAsync(role, true);
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.Permissions,
                CreationTime = DateTime.UtcNow
            };
        }
        [AppServiceAuthorize(PermissionNames.Role_Update)]
        public async Task<RoleDto> UpdateRoleAsync(RoleDto input)
        {
            var role = await roleRepository.GetAsync(r => r.Id == input.Id);
            role.Name = input.Name;
            role.Description = input.Description;
            role.Permissions = input.Permissions;

            await roleRepository.UpdateAsync(role);
            return input;
        }
        [AppServiceAuthorize(PermissionNames.Role_Delete)]
        public async Task DeleteRoleAsync(Guid id)
        {
            var userRoles = await userRoleRepository.GetListAsync(ur => ur.RoleId == id);
            if (userRoles.Count > 0)
            {
                throw new ValidationException($"Cannot delete role as it is assigned to one or more users. User ids: {string.Join(", ", userRoles.Select(u => u.UserId))}");
            }
            await roleRepository.DeleteAsync(r => r.Id == id, true);
        }

        public List<string> GetAllPermissions()
        {
            return PermissionNames.Permissions.ToList();
        }
    }
}
