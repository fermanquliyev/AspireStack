using AspireStack.Domain.Entities.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Infrastructure.Jwt
{
    public interface IJwtTokenHandler
    {
        string GenerateJwtToken(User user);
    }
}
