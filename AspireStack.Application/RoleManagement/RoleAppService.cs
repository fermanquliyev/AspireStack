using AspireStack.Application.AppService;
using AspireStack.Application.Security;
using AspireStack.Application.RoleManagement.DTOs;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using System.ComponentModel.DataAnnotations;

namespace AspireStack.Application.UserManagement
{
    [AppServiceAuthorize]
    public class RoleAppService : IAppService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepository<User, Guid> userRepository;
        private readonly IRepository<Role, Guid> roleRepository;
        private readonly IRepository<UserRole, string> userRoleRepository;
        private readonly ICurrentUser currentUser;

        public RoleAppService(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            this.unitOfWork = unitOfWork;
            this.userRepository = unitOfWork.Repository<User, Guid>();
            this.roleRepository = unitOfWork.Repository<Role, Guid>();
            this.userRoleRepository = unitOfWork.Repository<UserRole, string>();
            this.currentUser = currentUser;
        }

        public async Task<RoleDto> GetRoleAsync(Guid id)
        {
            var role = await roleRepository.GetAsync(r => r.Id == id);
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.Permissions
            };
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await roleRepository.GetListAsync(r => true);
            return roles.Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.Permissions
            }).ToList();
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto input)
        {
            var role = new Role
            {
                Name = input.Name,
                Description = input.Description,
                Permissions = input.Permissions,
                CreationTime = DateTime.UtcNow,
                CreatorId = currentUser.Id
            };

            await roleRepository.InsertAsync(role,true);
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = role.Permissions
            };
        }

        public async Task<RoleDto> UpdateRoleAsync(RoleDto input)
        {
            var role = await roleRepository.GetAsync(r => r.Id == input.Id);
            role.Name = input.Name;
            role.Description = input.Description;
            role.Permissions = input.Permissions;
            role.LastModificationTime = DateTime.UtcNow;
            role.LastModifierId = currentUser.Id;

            await roleRepository.UpdateAsync(role, true);
            return input;
        }

        public async Task DeleteRoleAsync(Guid id)
        {
            var userRoles = await userRoleRepository.GetListAsync(ur => ur.RoleId == id);
            if (userRoles.Count > 0)
            {
                throw new ValidationException($"Cannot delete role as it is assigned to one or more users. User ids: {string.Join(", ", userRoles.Select(u => u.UserId))}");
            }
            await roleRepository.DeleteAsync(r => r.Id == id, true);
        }
    }
}
