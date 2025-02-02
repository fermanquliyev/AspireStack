using AspireStack.Domain.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AspireStack.Infrastructure.Jwt
{
    public class CurrentUser<TKey> : ICurrentUser<TKey> where TKey : struct, IParsable<TKey>
    {
        private readonly IEnumerable<Claim> claims;
        private readonly bool _isAuthenticated;

        public CurrentUser(IHttpContextAccessor httpContext)
        {
            this.claims = httpContext.HttpContext?.User.Claims ?? new List<Claim>();
            this._isAuthenticated = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        }
        public TKey? Id
        {
            get
            {
                return TKey.TryParse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, null, out var result) ? result : null;
            }
        }

        public string? Username => claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        public string? Email => claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        public string? FirstName => claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;

        public string? LastName => claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

        public string? PhoneNumber => claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone)?.Value;

        public string[]? Roles => claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
        public string[]? Permissions => claims.Where(x => x.Type == ClaimTypes.Actor).Select(x => x.Value).ToArray();

        public bool IsAuthenticated => _isAuthenticated;
    }

    public class CurrentUser : ICurrentUser
    {
        private readonly IEnumerable<Claim> claims;
        private readonly bool _isAuthenticated;

        public CurrentUser(IHttpContextAccessor httpContext)
        {
            this.claims = httpContext.HttpContext?.User.Claims ?? new List<Claim>();
            this._isAuthenticated = httpContext.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        }
        public Guid? Id
        {
            get
            {
                return Guid.TryParse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, null, out var result) ? result : null;
            }
        }

        public string? Username => claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        public string? Email => claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        public string? FirstName => claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;

        public string? LastName => claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

        public string? PhoneNumber => claims.FirstOrDefault(x => x.Type == ClaimTypes.MobilePhone)?.Value;

        public string[]? Roles => claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
        public string[]? Permissions => claims.Where(x => x.Type == ClaimTypes.Actor).Select(x => x.Value).ToArray();

        public bool IsAuthenticated => _isAuthenticated;
    }
}
