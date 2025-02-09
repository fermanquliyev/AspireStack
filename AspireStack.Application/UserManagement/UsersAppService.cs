using AspireStack.Application.AppService;
using AspireStack.Application.AppService.DTOs;
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
        public async Task<PagedResult<UserDto>> GetUsersAsync(GetUsersInput usersInput)
        {
            var query = userRepository.GetQueryable();
            if (!string.IsNullOrEmpty(usersInput.Filter))
            {
                query = query.Where(u => u.FirstName.Contains(usersInput.Filter) || u.LastName.Contains(usersInput.Filter) || u.Email.Contains(usersInput.Filter));
            }
            var totalCount = await unitOfWork.AsyncQueryableExecuter.CountAsync(query);
            query = query.OrderBy(u => u.Id).Skip((usersInput.Page-1)*usersInput.PageSize).Take(usersInput.PageSize);
            var items = await unitOfWork.AsyncQueryableExecuter.ToListAsync(query.Select(UserDto.FromUser));

            return new PagedResult<UserDto>(items, totalCount);
        }

        [AppServiceAuthorize(PermissionNames.User_View)]
        public async Task<CreateEditUserDto> GetUserByIdAsync(Guid id)
        {
            var userQuery = unitOfWork.Repository<User, Guid>().WithDetails(x => x.Roles).Where(x => x.Id == id);
            var user = await unitOfWork.AsyncQueryableExecuter.FirstOrDefaultAsync(userQuery);
            if (user == null)
            {
                throw new ApplicationException("User not found.");
            }
            var resultDto = CreateEditUserDto.FromUser(user);

            if(user.CreatorId.HasValue && user.CreatorId != Guid.Empty)
            {
                var creator = await userRepository.FindAsync(u => u.Id == user.CreatorId);
                if (creator != null)
                resultDto.CreatedUser = UserDto.FromUser(creator);
            }

            if (user.LastModifierId.HasValue && user.LastModifierId != Guid.Empty)
            {
                var lastModifier = await userRepository.FindAsync(u => u.Id == user.LastModifierId);
                if (lastModifier != null)
                    resultDto.LastModifiedUser = UserDto.FromUser(lastModifier);
            }

            return resultDto;
        }

        [AppServiceAuthorize(PermissionNames.User_Create)]
        public async Task CreateUserAsync(CreateEditUserDto input)
        {
            var user = new User
            {
                Username = input.Username,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                PhoneNumber = input.PhoneNumber
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
        public async Task UpdateUserAsync(CreateEditUserDto input)
        {
            var user = await userRepository.FindAsync(u => u.Id == input.Id);
            if (user != null)
            {
                user.Username = input.Username;
                user.FirstName = input.FirstName;
                user.LastName = input.LastName;
                user.PhoneNumber = input.PhoneNumber;
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
