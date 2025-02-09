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
        public string GenerateUserToken(User user, TokenParameters parameters)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(parameters.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Name, user.FirstName),
                    new(JwtRegisteredClaimNames.Email, user.Email),
                    new(JwtRegisteredClaimNames.FamilyName, user.LastName),
                    new(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.AuthTime, DateTime.Now.ToString()),
                    new(CustomClaimTypes.EmailVerified, user.EmailVerified.ToString())
                };

            if (user.Roles != null && user.Roles.Count != 0)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new(CustomClaimTypes.Role, role.Role.Name));
                    if (role.Role.Permissions != null && role.Role.Permissions.Length != 0)
                    {
                        foreach (var permission in role.Role.Permissions)
                        {
                            claims.Add(new(CustomClaimTypes.Permission, permission));
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
