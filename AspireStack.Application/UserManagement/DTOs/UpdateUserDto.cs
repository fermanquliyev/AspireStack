namespace AspireStack.Application.UserManagement.DTOs
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Guid> RoleIds { get; set; }
    }
}
