
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AspireStack.Domain.Entities.UserManagement
{
    public class User : IdentityUser<Guid>, IAuditedEntity, IValidatedEntity, IEntity<Guid>
    {
        public User()
        {
            
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool EmailVerified { get; set; }
        public bool PhoneNumberVerified { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                throw new ValidationException("FirstName is required");
            }

            if (string.IsNullOrEmpty(LastName))
            {
                throw new ValidationException("LastName is required");
            }

            if (string.IsNullOrEmpty(Email))
            {
                throw new ValidationException("Email is required");
            }

            if (!new EmailAddressAttribute().IsValid(Email))
            {
                throw new ValidationException("Email is not valid");
            }

            if (string.IsNullOrEmpty(UserName))
            {
                throw new ValidationException("Username is required");
            }

            if (string.IsNullOrEmpty(PasswordHash))
            {
                throw new ValidationException("Password is required");
            }
        }
    }
}
