using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Entities.UserManagement
{
    /// <summary>
    /// Contains all the permission names used in the application.
    /// </summary>
    public static class PermissionNames
    {
        #region User Management
        public const string User_Create = "UserManagement.Users.Create";
        public const string User_Update = "UserManagement.Users.Update";
        public const string User_Delete = "UserManagement.Users.Delete";
        public const string User_View = "UserManagement.Users.View";
        public const string Role_Create = "UserManagement.Roles.Create";
        public const string Role_Update = "UserManagement.Roles.Update";
        public const string Role_Delete = "UserManagement.Roles.Delete";
        public const string Role_View = "UserManagement.Roles.View";
        #endregion

        public enum Permission
        {
            User_Create,
            User_Update,
            User_Delete,
            User_View,
            Role_Create,
            Role_Update,
            Role_Delete,
            Role_View
        }

        public static readonly IReadOnlyDictionary<Permission, string> PermissionStrings = new Dictionary<Permission, string>
            {
                { Permission.User_Create, User_Create },
                { Permission.User_Update, User_Update },
                { Permission.User_Delete, User_Delete },
                { Permission.User_View, User_View },
                { Permission.Role_Create, Role_Create },
                { Permission.Role_Update, Role_Update },
                { Permission.Role_Delete, Role_Delete },
                { Permission.Role_View, Role_View }
            };

        /// <summary>
        /// Gets the permission string by enum.
        /// </summary>
        public static string GetPermissionString(Permission permission) => PermissionStrings[permission];

        public static Permission GetPermissionEnum(string permission)
        {
            return PermissionStrings.FirstOrDefault(x => x.Value == permission).Key;
        }
    }
}
