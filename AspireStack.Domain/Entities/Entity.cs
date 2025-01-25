using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Entities
{
    public abstract class Entity<T> where T : struct
    {
        public required T Id { get; set; }
    }
}
