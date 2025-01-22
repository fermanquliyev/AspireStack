using GeoPlanner.Domain.Entities.LocationManagement;
using GeoPlanner.Domain.Shared.Entities.RouteManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPlanner.Domain.Entities.RouteManagement
{
    public class Route: Entity<Guid>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public required Guid UserId { get; set; }
        public required TransportationMethod TransportationMethod { get; set; }
    }
}
