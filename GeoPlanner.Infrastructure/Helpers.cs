using GeoPlanner.Domain.Entities.UserManagement;
using GeoPlanner.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoPlanner.Infrastructure
{
    public static class Helpers
    {
        public static bool AddAdminUser(this GeoPlannerDbContext dbContext, string username, string passwordHashed)
        {
            var dbUser = dbContext.Users.FirstOrDefault(u => u.Username == username);
            if (dbUser != null)
            {
                return false;
            }
            var adminUser = dbContext.Users.Add(new User
            {
                Id = Guid.Empty,
                Username = "admin",
                PasswordHashed = passwordHashed
            });
            return true;
        }
    }
}
