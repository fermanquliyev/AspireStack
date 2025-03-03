using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspireStack.Domain.Entities.UserManagement
{
    public class UserRole : IdentityUserRole<Guid>, IEntity<string>
    {
        [NotMapped]
        public string Id { get => $"{UserId}-{RoleId}"; set {} }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
