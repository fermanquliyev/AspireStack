using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Entities.UserManagement
{
    public class Role : IdentityRole<Guid>, IAuditedEntity, IValidatedEntity, IEntity<Guid>
    {
        public string? Description { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
        public virtual ICollection<RoleClaim> RoleClaims { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new ValidationException("Role name is required");
            }
        }
    }
}
