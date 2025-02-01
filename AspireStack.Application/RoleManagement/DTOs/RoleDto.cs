using AspireStack.Domain.Entities.UserManagement;

namespace AspireStack.Application.RoleManagement.DTOs
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Permissions { get; set; }
        public static RoleDto FromRole(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }
    }
}
