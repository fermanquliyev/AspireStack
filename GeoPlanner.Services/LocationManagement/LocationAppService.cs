using GeoPlanner.Application.AppService;
using GeoPlanner.Domain.Entities.LocationManagement;
using GeoPlanner.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPlanner.Application.LocationManagement
{
    public class LocationAppService : IAppService
    {
        private readonly IRepository<Location, Guid> repository;

        public LocationAppService(IRepository<Location, Guid> repository)
        {
            this.repository = repository;
        }

        public async Task<LocationDto> GetLocation(Guid id)
        {
            return await Task.FromResult(new LocationDto
            {
                Id = id,
                Name = "Test Location",
                Address = "",
                City = "",
                Country = "",
                UserId = Guid.NewGuid()
            });
        }


        public async Task<Location> CreateAsync(LocationDto locationDto)
        {
            return await repository.InsertAsync(locationDto.ToLocation());
        }
    }

    public class LocationDto
    {
        public Guid Id { get; set; }
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

        public Location ToLocation() => new Location
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Address = Address,
            City = City,
            State = State,
            Country = Country,
            PostalCode = PostalCode,
            Latitude = Latitude,
            Longitude = Longitude,
            UserId = UserId,
            RouteId = RouteId
        };
    }
}
