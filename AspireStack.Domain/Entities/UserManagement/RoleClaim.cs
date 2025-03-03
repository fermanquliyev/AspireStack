using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Entities.UserManagement
{
    public class RoleClaim: IdentityRoleClaim<Guid>, IEntity<int>
    {
    }
}
