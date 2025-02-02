using AspireStack.Domain.Entities.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Services
{
    public record TokenParameters(string Secret, string Issuer, string Audience, int ExpInDays = 1);

    public interface IUserTokenHandler
    {
        string GenerateUserToken(User user, TokenParameters parameters);
    }
}
