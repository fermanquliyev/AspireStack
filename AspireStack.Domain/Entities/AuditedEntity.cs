using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Entities
{
    public interface IAuditedEntity : ISoftDelete
    {
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public Guid? CreatorId { get; set; }
        public Guid? LastModifierId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
