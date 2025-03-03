using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Services;
using AspireStack.Domain.Shared.UserManagement;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspireStack.Infrastructure.Jwt
{
    public class JwtTokenHandler : IUserTokenHandler
    {
        public string GenerateUserToken(User user, List<Role> roles, List<RoleClaim> roleClaims, TokenParameters parameters)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(parameters.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Name, user.FirstName),
                    new(JwtRegisteredClaimNames.Email, user.Email),
                    new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                    new(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.AuthTime, DateTime.Now.ToString()),
                    new(CustomClaimTypes.EmailVerified, user.EmailVerified.ToString())
                };

            if (roles != null && roles.Count != 0)
            {
                foreach (var role in roles)
                {
                    claims.Add(new(CustomClaimTypes.Role, role.Name));
                    if (roleClaims.Any(c=>c.RoleId == role.Id))
                    {
                        foreach (var permission in roleClaims.Where(c => c.RoleId == role.Id))
                        {
                            claims.Add(new(CustomClaimTypes.Permission, permission.ClaimValue));
                        }
                    }
                }
            }

            var token = new JwtSecurityToken(
                issuer: parameters.Issuer,
                audience: parameters.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(parameters.ExpInDays),
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
