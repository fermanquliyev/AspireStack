
using System.ComponentModel.DataAnnotations;

namespace AspireStack.Domain.Entities.UserManagement
{
    public class User : Entity<Guid>, IAuditedEntity, IValidatedEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailVerified { get; set; }
        public bool PhoneNumberVerified { get; set; }
        public string Username { get; set; }
        public string PasswordHashed { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public List<Role> Roles { get; set; }

        public void AddRole(Role role)
        {
            if (Roles == null)
            {
                Roles = new List<Role>();
            }
            if (Roles.Any(r => r.Id == role.Id))
            {
                return;
            }
            Roles.Add(role);
        }

        public void RemoveRole(Guid roleId)
        {
            if (Roles == null)
            {
                return;
            }
            Roles.RemoveAll(r => r.Id == roleId);
        }

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

            if (string.IsNullOrEmpty(Username))
            {
                throw new ValidationException("Username is required");
            }

            if (string.IsNullOrEmpty(PasswordHashed))
            {
                throw new ValidationException("Password is required");
            }
        }
    }
}
