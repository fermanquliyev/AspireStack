using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Services;
using AspireStack.Domain.Shared.UserManagement;
using AspireStack.Infrastructure.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AspireStack.WebApi.Host.Authentication
{
    public static class ConfigureAuthentication
    {
        public static void AddAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(AddPolicies);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            builder.Services.AddIdentityApiEndpoints<User>()
                            .AddEntityFrameworkStores<AspireStackDbContext>();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            });
            builder.Services.AddOptions<BearerTokenOptions>(IdentityConstants.BearerScheme).Configure(options =>
            {
                options.BearerTokenExpiration = TimeSpan.FromDays(1);
                options.RefreshTokenExpiration = TimeSpan.FromDays(30);
            });
            builder.Services.AddSingleton<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory>();
        }

        /// <summary>
        /// Add policies for authorization. Each policy is based on a permission.
        /// </summary>
        /// <param name="authentication"></param>
        public static void AddPolicies(AuthorizationOptions authentication)
        {
            var permissionNames = PermissionNames.PermissionStrings;
            foreach (var permission in permissionNames)
            {
                authentication.AddPolicy(permission, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(context =>
                    {
                        return context.User.Claims.Where(x => x.Type == CustomClaimTypes.Permission).Any(x => x.Value == permission);
                    });
                });
            }
        }
    }
}
