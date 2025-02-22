using AspireStack.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace AspireStack.Infrastructure.Jwt
{
    /// <summary>
    /// Provides an implementation of <see cref="IUserPasswordHasher{T}"/> that uses the ASP.NET Core Identity password hashing functionality <see cref="IPasswordHasher<T>"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultPasswordHasher<T> : IUserPasswordHasher<T> where T : class
    {
        private readonly IPasswordHasher<T> passwordHasher;

        public DefaultPasswordHasher(IPasswordHasher<T> passwordHasher)
        {
            this.passwordHasher = passwordHasher;
        }
        public string HashPassword(T user, string password)
        {
            return passwordHasher.HashPassword(user, password);
        }

        public AspireStack.Domain.Shared.Enums.PasswordVerificationResult VerifyHashedPassword(T user, string hashedPassword, string providedPassword)
        {
            return passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword) switch
            {
                Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed => AspireStack.Domain.Shared.Enums.PasswordVerificationResult.Failed,
                Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success => AspireStack.Domain.Shared.Enums.PasswordVerificationResult.Success,
                Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded => AspireStack.Domain.Shared.Enums.PasswordVerificationResult.SuccessRehashNeeded,
                _ => throw new NotSupportedException()
            };
        }
    }
}
