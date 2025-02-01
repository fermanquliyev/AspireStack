using AspireStack.Domain.Entities.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Application.UserManagement.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public User ToUser()
        {
            return new User
            {
                Id = Id,
                Username = Username,
                Email = Email,
                PhoneNumber = PhoneNumber,
                FirstName = FirstName,
                LastName = LastName,
                CreationTime = CreationTime,
                LastModificationTime = LastModificationTime
            };
        }

        public static UserDto FromUser(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreationTime = user.CreationTime,
                LastModificationTime = user.LastModificationTime
            };
        }
    }
}
