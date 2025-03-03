namespace AspireStack.WebApi.Models
{
    public class UserInfoModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string[] Permissions { get; set; }
    }
}
