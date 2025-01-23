
namespace AspireStack.Domain.Entities.UserManagement
{
    public class User: Entity<Guid>
    {
        public string Username { get; set; }

        public string PasswordHashed { get; set; }
    }
}
