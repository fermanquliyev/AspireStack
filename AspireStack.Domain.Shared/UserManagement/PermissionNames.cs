using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Entities.UserManagement
{
    public static class PermissionNames
    {
        public const string User_Create = "UserManagement.Users.Create";
        public const string User_Update = "UserManagement.Users.Update";
        public const string User_Delete = "UserManagement.Users.Delete";
        public const string User_View = "UserManagement.Users.View";
        public const string Role_Create = "UserManagement.Roles.Create";
        public const string Role_Update = "UserManagement.Roles.Update";
        public const string Role_Delete = "UserManagement.Roles.Delete";
        public const string Role_View = "UserManagement.Roles.View";

        private static readonly Lazy<IReadOnlyList<string>> _permissions = new Lazy<IReadOnlyList<string>>(() => new List<string>
                    {
                        User_Create,
                        User_Update,
                        User_Delete,
                        User_View,
                        Role_Create,
                        Role_Update,
                        Role_Delete,
                        Role_View,
                    }.AsReadOnly());

        public static IReadOnlyList<string> Permissions => _permissions.Value;
    }
}
