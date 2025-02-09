using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using AspireStack.Domain.Shared.Enums;
using AspireStack.WebApi.DynamicRouteMapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
namespace AspireStack.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IUserTokenHandler tokenProvider, IOptions<TokenParameters> tokenParameterOptions, IUnitOfWork unitOfWork, IUserPasswordHasher<User> passwordHasher, ICurrentUser<Guid> currentUser)
        : ControllerBase
    {
        private readonly IUserTokenHandler tokenProvider = tokenProvider;
        private readonly TokenParameters tokenParameters = tokenParameterOptions.Value ?? throw new ArgumentNullException(nameof(tokenParameterOptions));
        private readonly IUnitOfWork unitOfWork = unitOfWork;
        private readonly IUserPasswordHasher<User> passwordHasher = passwordHasher;
        private readonly ICurrentUser<Guid> currentUser = currentUser;

        [HttpPost("login")]
        public async Task<ActionResult<WebApiResult<string>>> Login([FromBody] LoginRequest request)
        {
            var user = await AuthenticateUser(request);
            if (user == null)
            {
                return Unauthorized("Email or password is wrong. Try again.");
            }

            var token = tokenProvider.GenerateUserToken(user, tokenParameters);
            return Ok(new WebApiResult { Data = token, StatusCode = 200, Success = true });
        }

        [HttpPost("register")]
        public async Task<ActionResult<WebApiResult<Guid>>> Register([FromBody] RegisterRequest request)
        {
            var user = new User
            {
                Id = default,
                FirstName = string.Empty,
                LastName = string.Empty,
                Email = request.Email,
                PhoneNumber = string.Empty,
                Username = request.Email,
            };
            user.PasswordHashed = passwordHasher.HashPassword(user, request.Password);
            await unitOfWork.Repository<User, Guid>().InsertAsync(user);
            return Ok(new WebApiResult { Data = user.Id, StatusCode = 200, Success = true });
        }

#if DEBUG
        [HttpPost("currentUser")]
        [Authorize]
        public ActionResult<WebApiResult<object>> CurrentUser()
        {
            var currentUserId = currentUser.Id;
            var username = currentUser.Username;
            var email = currentUser.Email;
            var roles = currentUser.Roles;
            var permissions = currentUser.Permissions;
            var data = new { Id = currentUserId, Username = username, Email = email, Roles = roles, Permissions = permissions };
            return Ok(new WebApiResult { Data = data, StatusCode = 200, Success = true });
        }
#endif

        private async Task<User?> AuthenticateUser(LoginRequest request)
        {
            var userQuery = unitOfWork.Repository<User, Guid>().WithInnerDetails(x => x.Roles, x=> x.Role).Where(x => x.Email == request.Email);
            var user = await unitOfWork.AsyncQueryableExecuter.FirstOrDefaultAsync(userQuery);
            if (user is null)
            {
                return null;
            }
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHashed, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }
            if (result.HasFlag(PasswordVerificationResult.Success))
            {
                return user;
            };
            return null;
        }
    }
}
