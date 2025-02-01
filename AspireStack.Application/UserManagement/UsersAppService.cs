using AspireStack.Application.AppService;
using AspireStack.Application.Security;
using AspireStack.Application.UserManagement.DTOs;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;

namespace AspireStack.Application.UserManagement
{
    [AppServiceAuthorize]
    public class UsersAppService : IAppService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepository<User, Guid> userRepository;
        private readonly IRepository<Role, Guid> roleRepository;
        private readonly IRepository<UserRole, string> userRoleRepository;
        private readonly ICurrentUser currentUser;

        public UsersAppService(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            this.unitOfWork = unitOfWork;
            this.userRepository = unitOfWork.Repository<User, Guid>();
            this.roleRepository = unitOfWork.Repository<Role, Guid>();
            this.userRoleRepository = unitOfWork.Repository<UserRole, string>();
            this.currentUser = currentUser;
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var query = userRepository.GetQueryable().Select(UserDto.FromUser);
            return await unitOfWork.AsyncQueryableExecuter.ToListAsync(query);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await userRepository.FindAsync(u => u.Id == id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            return UserDto.FromUser(user);
        }

        public async Task CreateUserAsync(CreateUserDto input)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email
            };
            await userRepository.InsertAsync(user);

            if (input.RoleIds != null && input.RoleIds.Any())
            {
                foreach (var roleId in input.RoleIds)
                {
                    var roleExists = await roleRepository.FindAsync(r => r.Id == roleId);
                    if (roleExists!=null)
                    {
                        var userRole = new UserRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            RoleId = roleId
                        };
                        await userRoleRepository.InsertAsync(userRole);
                    }
                }
            }

            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UpdateUserDto input)
        {
            var user = await userRepository.FindAsync(u => u.Id == input.Id);
            if (user != null)
            {
                user.FirstName = input.FirstName;
                user.LastName = input.LastName;
                // Email cannot be updated.
                await userRepository.UpdateAsync(user);

                var existingUserRoles = await userRoleRepository.GetListAsync(ur => ur.UserId == user.Id);
                var existingRoleIds = existingUserRoles.Select(ur => ur.RoleId).ToList();

                var rolesToAdd = input.RoleIds.Except(existingRoleIds).ToList();
                var rolesToRemove = existingRoleIds.Except(input.RoleIds).ToList();

                foreach (var roleId in rolesToAdd)
                {
                    var roleExists = await roleRepository.FindAsync(r => r.Id == roleId);
                    if (roleExists != null)
                    {
                        var userRole = new UserRole
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            RoleId = roleId
                        };
                        await userRoleRepository.InsertAsync(userRole);
                    }
                }

                foreach (var roleId in rolesToRemove)
                {
                    await userRoleRepository.DeleteAsync(ur => ur.UserId == user.Id && ur.RoleId == roleId);
                }

                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await userRepository.FindAsync(u => u.Id == id);
            if (user != null)
            {
                await userRoleRepository.DeleteAsync(ur => ur.UserId == id);
                await userRepository.DeleteAsync(user);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
