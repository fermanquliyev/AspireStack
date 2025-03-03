using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Shared.UserManagement;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class UserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<User>
{
    private readonly IServiceProvider serviceProvider;

    public UserClaimsPrincipalFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    public async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(CustomClaimTypes.EmailVerified, user.EmailVerified.ToString())
        };

        var userRoles = unitOfWork.Repository<UserRole, string>().GetQueryableAsNoTracking().Where(x => x.UserId == user.Id);
        var roles = unitOfWork.Repository<Role, Guid>().GetQueryableAsNoTracking();
        var roleClaimsQuery = unitOfWork.Repository<RoleClaim, int>().GetQueryableAsNoTracking();

        var roleClaims = await unitOfWork.AsyncQueryableExecuter.ToListAsync(
        userRoles.Join(roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
            .Join(roleClaimsQuery, r => r.Id, rc => rc.RoleId, (r, rc) => rc));
        roleClaims.ForEach(x => claims.Add(new Claim(x.ClaimType, x.ClaimValue)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme));
    }
}