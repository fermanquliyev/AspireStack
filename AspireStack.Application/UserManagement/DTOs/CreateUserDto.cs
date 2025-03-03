using AspireStack.Domain.Entities.UserManagement;
using System.ComponentModel.DataAnnotations;

namespace AspireStack.Application.UserManagement.DTOs
{
    public class CreateEditUserDto
    {
        public Guid? Id { get; set; }
        [Required]
        [MinLength(5)]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public UserDto? CreatedUser { get; set; }
        public UserDto? LastModifiedUser { get; set; }

        [MinLength(1)]
        public List<Guid> RoleIds { get; set; }

        public static CreateEditUserDto FromUser(User user, IEnumerable<Guid> roleIds)
        {
            return new CreateEditUserDto
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreationTime = user.CreationTime,
                LastModificationTime = user.LastModificationTime,
                RoleIds = roleIds.ToList()
            };
        }
    }
}
