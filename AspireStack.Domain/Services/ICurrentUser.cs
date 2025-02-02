using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Services
{
    /// <summary>
    /// Represents the current user.
    /// </summary>
    /// <typeparam name="TKey">Type of Id of user.</typeparam>
    public interface ICurrentUser<TKey> where TKey: struct, IParsable<TKey>
    {
        public TKey? Id { get; }
        public string? Username { get; }
        public string? Email { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public string? PhoneNumber { get; }
        public string[]? Roles { get; }
        public string[]? Permissions { get; }
        public bool IsAuthenticated { get; }
    }

    /// <summary>
    /// Represents the current user. User's Id is of type Guid.
    /// </summary>
    public interface ICurrentUser : ICurrentUser<Guid>
    {
    }
}
