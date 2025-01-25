using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspireStack.Domain.Services
{
    public interface ICurrentUser<TKey> where TKey : struct
    {
        public TKey? Id { get; }
        public string? Username { get; }
        public string? Email { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public string? PhoneNumber { get; }
        public string[]? Roles { get; }
        public bool IsAuthenticated { get; }
    }
}
