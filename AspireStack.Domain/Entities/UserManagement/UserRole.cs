using System.ComponentModel.DataAnnotations.Schema;

namespace AspireStack.Domain.Entities.UserManagement
{
    public class UserRole: Entity<string>
    {
        [NotMapped]
        override public string Id
        {
            get
            {
                return $"{UserId}_{RoleId}";
            }
            set
            {
                _ = value;
            }
        }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
