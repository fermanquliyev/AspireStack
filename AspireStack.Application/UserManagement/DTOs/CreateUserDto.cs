namespace AspireStack.Application.UserManagement.DTOs
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Guid> RoleIds { get; set; }
    }
}
