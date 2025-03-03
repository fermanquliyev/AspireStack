using AspireStack.Application.AppService;
using AspireStack.Application.Security;
using AspireStack.Application.RoleManagement.DTOs;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using System.ComponentModel.DataAnnotations;
using AspireStack.Application.AppService.DTOs;
using System.Data;
using Microsoft.AspNetCore.Identity;
using AspireStack.Domain.Shared.UserManagement;

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
            var role = await this.CacheClient.GetAsync<RoleDto>($"role_{id}");
            if (role is not null)
            {
                return role;
            }

            var roleEntity = await AsyncExecuter.FirstOrDefaultAsync(roleRepository.GetQueryableAsNoTracking().Where(r => r.Id == id));
            if (roleEntity == null)
            {
                throw new ValidationException(L("RoleNotFound"));
            }
            var permissions = await AsyncExecuter.ToListAsync(UnitOfWork.Repository<RoleClaim, int>()
                .GetQueryableAsNoTracking().Where(r => r.RoleId == id && r.ClaimType == CustomClaimTypes.Permission));
            role = new RoleDto
            {
                Id = roleEntity.Id,
                Name = roleEntity.Name,
                Description = roleEntity.Description,
                CreationTime = roleEntity.CreationTime,
                LastModificationTime = roleEntity.LastModificationTime,
                Permissions = permissions != null ? permissions.Select(p => p.ClaimValue!).ToArray() : []
            };
            await this.CacheClient.SetAsync($"role_{id}", role, TimeSpan.FromMinutes(1));
            return role;
        }

        [AppServiceAuthorize(PermissionNames.Role_View)]
        public async Task<List<RoleDto>> GetAllRoles()
        {
            var roles = await this.CacheClient.GetAsync<List<RoleDto>>("roles");
            if (roles is not null)
            {
                return roles;
            }

            var query = roleRepository.GetQueryableAsNoTracking()
            .OrderBy(x => x.Name)
            .Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
            });
            roles = await AsyncExecuter.ToListAsync(query);
            var allPermissions = await AsyncExecuter.ToListAsync(UnitOfWork.Repository<RoleClaim, int>()
                .GetQueryableAsNoTracking().Where(r => r.ClaimType == CustomClaimTypes.Permission));
            foreach (var role in roles)
            {
                role.Permissions = allPermissions.Where(p => p.RoleId == role.Id).Select(p => p.ClaimValue!).ToArray();
            }
            await this.CacheClient.SetAsync("roles", roles, TimeSpan.FromMinutes(5));
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
            };

            await roleRepository.InsertAsync(role, true);

            foreach (var perm in input.Permissions)
            {
                await UnitOfWork.Repository<RoleClaim, int>().InsertAsync(new RoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = CustomClaimTypes.Permission,
                    ClaimValue = perm,
                });
            }

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = input.Permissions,
                CreationTime = DateTime.UtcNow
            };
            await this.CacheClient.SetAsync($"role_{role.Id}", roleDto, TimeSpan.FromMinutes(1));
            return roleDto;
        }
        [AppServiceAuthorize(PermissionNames.Role_Update)]
        public async Task<RoleDto> UpdateRoleAsync(RoleDto input)
        {
            var role = await roleRepository.GetAsync(r => r.Id == input.Id);
            if (role == null)
            {
                throw new ValidationException("Role not found");
            }
            role.Name = input.Name;
            role.Description = input.Description;
            
            await UnitOfWork.Repository<RoleClaim, int>().DeleteDirectAsync(r => r.RoleId == role.Id);

            foreach (var perm in input.Permissions)
            {
                await UnitOfWork.Repository<RoleClaim, int>().InsertAsync(new RoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = CustomClaimTypes.Permission,
                    ClaimValue = perm,
                });
            }


            await roleRepository.UpdateAsync(role);
            await this.CacheClient.SetAsync($"role_{role.Id}", input, TimeSpan.FromMinutes(1));
            return input;
        }
        [AppServiceAuthorize(PermissionNames.Role_Delete)]
        public async Task DeleteRoleAsync(Guid id)
        {
            var userRoles = await userRoleRepository.GetListAsync(ur => ur.RoleId == id);
            if (userRoles.Count > 0)
            {
                throw new ValidationException(L("CantDeleteRoleWithUsers", string.Join(", ", userRoles.Select(u => u.UserId))));
            }
            await roleRepository.DeleteAsync(r => r.Id == id, true);
            await UnitOfWork.Repository<RoleClaim, int>().DeleteDirectAsync(r => r.RoleId == id);
            await this.CacheClient.RemoveAsync($"role_{id}");
        }

        public List<string> GetAllPermissions()
        {
            return PermissionNames.PermissionStrings.ToList();
        }
    }
}
