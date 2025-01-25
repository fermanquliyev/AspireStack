using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Repository;
using AspireStack.Domain.Services;
using AspireStack.Infrastructure.Jwt;
using AspireStack.WebApi.DynamicRouteMapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
namespace AspireStack.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IJwtTokenHandler tokenProvider, IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, ICurrentUser<Guid> currentUser)
        : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await AuthenticateUser(request);
            if (user == null)
            {
                return Unauthorized("Email or password is wrong. Try again.");
            }

            var token = tokenProvider.GenerateJwtToken(user);
            return Ok(new WebApiResult { Data = token, StatusCode = 200, Success = true });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
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
        public IActionResult CurrentUser()
        {
            var currentUserId = currentUser.Id;
            var username = currentUser.Username;
            var email = currentUser.Email;
            var data = new { Id = currentUserId, Username = username, Email = email };
            return Ok(new WebApiResult { Data = data, StatusCode = 200, Success = true });
        }
#endif

        private async Task<User?> AuthenticateUser(LoginRequest request)
        {
            var user = await unitOfWork.Repository<User, Guid>().FindAsync(x => x.Email == request.Email);
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
