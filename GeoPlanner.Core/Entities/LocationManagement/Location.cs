using GeoPlanner.Domain.Entities.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPlanner.Domain.Entities.LocationManagement
{
    public class Location: Entity<Guid>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? State { get; set; }
        public required string Country { get; set; }
        public string? PostalCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Navigation properties
        public required Guid UserId { get; set; }
        public Guid? RouteId { get; set; }
    }
}
