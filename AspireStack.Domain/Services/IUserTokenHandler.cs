using AspireStack.Domain.Entities.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Services
{
    public class TokenParameters
    {
        public string Secret { get; set; } 
        public string Issuer { get; set; } 
        public string Audience { get; set; }
        public int ExpInDays { get; set; } = 1;
    }

    public interface IUserTokenHandler
    {
        string GenerateUserToken(User user, List<Role> roles, List<RoleClaim> roleClaims, TokenParameters parameters);
    }
}
