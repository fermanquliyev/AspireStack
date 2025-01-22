using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPlanner.Domain.Entities
{
    public abstract class Entity<T>
    {
        public required T Id { get; set; }
    }
}
