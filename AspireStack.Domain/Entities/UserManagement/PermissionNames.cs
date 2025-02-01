using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Entities.UserManagement
{
    public static class PermissionNames
    {
        private static readonly Lazy<IReadOnlyList<string>> _permissions = new Lazy<IReadOnlyList<string>>(() => new List<string>
            {
                "UserManagement.Users.Create",
                "UserManagement.Users.Update",
                "UserManagement.Users.Delete",
                "UserManagement.Users.View",
                "UserManagement.Roles.Create",
                "UserManagement.Roles.Update",
                "UserManagement.Roles.Delete",
                "UserManagement.Roles.View",
            }.AsReadOnly());

        public static IReadOnlyList<string> Permissions => _permissions.Value;
    }
}
