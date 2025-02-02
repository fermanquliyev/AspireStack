using AspireStack.Application.AppService;
using AspireStack.Application.Security;
using AspireStack.Application.UserManagement.DTOs;
using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using AspireStack.Domain.Shared.Enums;
using System.ComponentModel.DataAnnotations;

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
        private readonly IUserPasswordHasher<User> userPasswordHasher;

        public UsersAppService(IUnitOfWork unitOfWork, ICurrentUser currentUser, IUserPasswordHasher<User> userPasswordHasher)
        {
            this.unitOfWork = unitOfWork;
            this.userRepository = unitOfWork.Repository<User, Guid>();
            this.roleRepository = unitOfWork.Repository<Role, Guid>();
            this.userRoleRepository = unitOfWork.Repository<UserRole, string>();
            this.currentUser = currentUser;
            this.userPasswordHasher = userPasswordHasher;
        }

        [AppServiceAuthorize(PermissionNames.User_View)]
        public async Task<List<UserDto>> GetUsersAsync()
        {
            var query = userRepository.GetQueryable().Select(UserDto.FromUser);
            return await unitOfWork.AsyncQueryableExecuter.ToListAsync(query);
        }

        [AppServiceAuthorize(PermissionNames.User_View)]
        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await userRepository.FindAsync(u => u.Id == id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            return UserDto.FromUser(user);
        }

        [AppServiceAuthorize(PermissionNames.User_Create)]
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
            var randomPassword = GenerateRandomPassword();
            var passwordHash = userPasswordHasher.HashPassword(user, randomPassword);
            user.PasswordHashed = passwordHash;

            if (input.RoleIds != null && input.RoleIds.Any())
            {
                foreach (var roleId in input.RoleIds)
                {
                    var role = await roleRepository.FindAsync(r => r.Id == roleId);
                    if (role != null)
                    {
                        user.AddRole(role);
                    }
                }
            }

            await unitOfWork.SaveChangesAsync();
        }

        [AppServiceAuthorize(PermissionNames.User_Update)]
        public async Task UpdateUserAsync(UpdateUserDto input)
        {
            var user = await userRepository.FindAsync(u => u.Id == input.Id);
            if (user != null)
            {
                user.FirstName = input.FirstName;
                user.LastName = input.LastName;
                // Email cannot be updated.

                var existingUserRoles = await userRoleRepository.GetListAsync(ur => ur.UserId == user.Id);
                var existingRoleIds = existingUserRoles.Select(ur => ur.RoleId).ToList();

                var rolesToAdd = input.RoleIds.Except(existingRoleIds).ToList();
                var rolesToRemove = existingRoleIds.Except(input.RoleIds).ToList();

                foreach (var roleId in rolesToAdd)
                {
                    var role = await roleRepository.FindAsync(r => r.Id == roleId);
                    if (role != null)
                    {
                        user.AddRole(role);
                    }
                }

                foreach (var roleId in rolesToRemove)
                {
                    user.RemoveRole(roleId);
                }

                await unitOfWork.SaveChangesAsync();
            }
        }

        [AppServiceAuthorize(PermissionNames.User_Update)]
        public async Task ResetPasswordAsync(Guid id)
        {
            var user = await userRepository.FindAsync(u => u.Id == id);
            if (user != null)
            {
                string randomPassword = GenerateRandomPassword();
                await ChangePasswordAsync(user.Id, randomPassword);
            }
        }

        private static string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        public async Task UpdateCurrentUserAsync(UpdateCurrentUserDto input)
        {
            var user = await userRepository.FindAsync(u => u.Id == currentUser.Id);
            if (user != null)
            {
                user.FirstName = input.FirstName;
                user.LastName = input.LastName;
                await userRepository.UpdateAsync(user);
                await unitOfWork.SaveChangesAsync();
                await ChangePasswordAsync(user.Id, input.NewPassword, input.CurrentPassword);
            }
        }

        private async Task ChangePasswordAsync(Guid id, string newPassword, string? currentPassword = null)
        {
            var user = await userRepository.FindAsync(u => u.Id == id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(currentPassword))
                {
                    var passwordVerificationResult = userPasswordHasher.VerifyHashedPassword(user, user.PasswordHashed, currentPassword);
                    if (passwordVerificationResult != PasswordVerificationResult.Success)
                    {
                        throw new ValidationException("Current password is incorrect.");
                    }
                }

                var passwordHash = userPasswordHasher.HashPassword(user, newPassword);
                user.PasswordHashed = passwordHash;
                await userRepository.UpdateAsync(user);
                await unitOfWork.SaveChangesAsync();
            }
        }

        [AppServiceAuthorize(PermissionNames.User_Delete)]
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
