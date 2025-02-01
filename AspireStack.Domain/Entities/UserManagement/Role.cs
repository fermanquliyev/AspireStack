using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Entities.UserManagement
{
    public class Role : Entity<Guid>, IAuditedEntity, IValidatedEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string[] Permissions { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ValidationException("Role name is required");
            }

            if (this.Permissions == null || this.Permissions.Length == 0)
            {
                throw new ValidationException("Role permissions are required");
            }

            foreach (var permission in this.Permissions)
            {
                if (!PermissionNames.Permissions.Contains(permission))
                {
                    throw new ValidationException($"Invalid permission {permission}");
                }
            }
        }
    }
}
