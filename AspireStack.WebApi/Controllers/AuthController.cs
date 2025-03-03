using AspireStack.Domain.Entities.UserManagement;
using AspireStack.Domain.Localization;
using AspireStack.Domain.Services;
using AspireStack.WebApi.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;

[ApiController]
[Route("Auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailSender<User> _emailSender;
    private readonly LinkGenerator linkGenerator;
    private readonly ICurrentUser currentUser;
    private readonly ILocalizationProvider localizationProvider;
    private readonly BearerTokenOptions bearerTokenOptions;
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender<User> emailSender,
        IOptions<BearerTokenOptions> bearerTokenOptions,
        LinkGenerator linkGenerator,
        ICurrentUser currentUser,
        ILocalizationProvider localizationProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        this.linkGenerator = linkGenerator;
        this.currentUser = currentUser;
        this.localizationProvider = localizationProvider;
        this.bearerTokenOptions = bearerTokenOptions.Value;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(Ok),200)]
    [ProducesResponseType(typeof(ValidationProblem), 400)]
    public async Task<Results<Ok, ValidationProblem>> Register([FromBody] RegisterRequest registration)
    {
        if (!_userManager.SupportsUserEmail)
            return TypedResults.ValidationProblem([new KeyValuePair<string, string[]>("Error", ["Email support is required."])]);

        if (string.IsNullOrEmpty(registration.Email) || !_emailAddressAttribute.IsValid(registration.Email))
            return CreateValidationProblem(IdentityResult.Failed(_userManager.ErrorDescriber.InvalidEmail(registration.Email)));

        var user = new User();
        await _userManager.SetUserNameAsync(user, registration.Email);
        await _userManager.SetEmailAsync(user, registration.Email);
        var result = await _userManager.CreateAsync(user, registration.Password);

        if (!result.Succeeded)
            return CreateValidationProblem(result);

        return TypedResults.Ok();
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AccessTokenResponse), 200)]
    [ProducesResponseType(typeof(ProblemHttpResult), 401)]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login([FromBody] LoginRequest login)
    {
        _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
        var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, true, true);
        if (result.RequiresTwoFactor)
        {
            if (!string.IsNullOrEmpty(login.TwoFactorCode))
            {
                result = await _signInManager.TwoFactorAuthenticatorSignInAsync(login.TwoFactorCode, true, true);
            }
            else if (!string.IsNullOrEmpty(login.TwoFactorRecoveryCode))
            {
                result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(login.TwoFactorRecoveryCode);
            }
        }

        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        // The signInManager already produced the needed response in the form of a bearer token.
        return TypedResults.Empty;
    }

    [HttpPost("refresh")]
    public async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> Refresh([FromBody] RefreshRequest refreshRequest)
    {
        var refreshTokenProtector = bearerTokenOptions.RefreshTokenProtector;
        var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

        // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
        if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
            DateTime.UtcNow >= expiresUtc ||
            await _signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not User user)

        {
            return TypedResults.Challenge();
        }

        var newPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
        return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            return Ok(); // Avoid disclosing user existence

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        await _emailSender.SendPasswordResetCodeAsync(user, request.Email, HtmlEncoder.Default.Encode(token));
        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return BadRequest("Invalid request.");

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpPost("resendConfirmationEmail")]
    public async Task<Ok> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest resendRequest)
    {
        if (await _userManager.FindByEmailAsync(resendRequest.Email) is not { } user)
        {
            return TypedResults.Ok();
        }

        await SendConfirmationEmailAsync(user, resendRequest.Email);
        return TypedResults.Ok();
    }

    [HttpGet("confirmEmail")]
    public async Task<Results<ContentHttpResult, UnauthorizedHttpResult>> ConfirmEmail([FromQuery] string userId, [FromQuery] string code, [FromQuery] string? changedEmail)
    {
        if (await _userManager.FindByIdAsync(userId) is not { } user)
        {
            // We could respond with a 404 instead of a 401 like Identity UI, but that feels like unnecessary information.
            return TypedResults.Unauthorized();
        }

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return TypedResults.Unauthorized();
        }

        IdentityResult result;

        if (string.IsNullOrEmpty(changedEmail))
        {
            result = await _userManager.ConfirmEmailAsync(user, code);
        }
        else
        {
            // As with Identity UI, email and user name are one and the same. So when we update the email,
            // we need to update the user name.
            result = await _userManager.ChangeEmailAsync(user, changedEmail, code);

            if (result.Succeeded)
            {
                result = await _userManager.SetUserNameAsync(user, changedEmail);
            }
        }

        if (!result.Succeeded)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Text("Thank you for confirming your email.");
    }

    [Authorize]
    [HttpGet("info")]
    public ActionResult<UserInfoModel> GetUserInfo()
    {
        var userInfo = new UserInfoModel
        {
            Email = currentUser.Email,
            FirstName = currentUser.FirstName,
            LastName = currentUser.LastName,
            PhoneNumber = currentUser.PhoneNumber,
            UserName = currentUser.Username,
            Permissions = currentUser.Permissions.ToArray()
        };

        return Ok(userInfo);
    }

    [Authorize]
    [HttpPost("info")]
    public async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>> PostUserInfo([FromBody] InfoRequest infoRequest)
    {
        if (await _userManager.GetUserAsync(HttpContext.User) is not { } user)
        {
            return TypedResults.NotFound();
        }

        if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !_emailAddressAttribute.IsValid(infoRequest.NewEmail))
        {
            return CreateValidationProblem(IdentityResult.Failed(_userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)));
        }

        if (!string.IsNullOrEmpty(infoRequest.NewPassword))
        {
            if (string.IsNullOrEmpty(infoRequest.OldPassword))
            {
                return CreateValidationProblem("OldPasswordRequired",
                    "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return CreateValidationProblem(changePasswordResult);
            }
        }

        if (!string.IsNullOrEmpty(infoRequest.NewEmail))
        {
            var email = await _userManager.GetEmailAsync(user);

            if (email != infoRequest.NewEmail)
            {
                await SendConfirmationEmailAsync(user, infoRequest.NewEmail, isChange: true);
            }
        }

        return TypedResults.Ok(await CreateInfoResponseAsync(user, _userManager));
    }

    async Task SendConfirmationEmailAsync(User user, string email, bool isChange = false)
    {
        var code = isChange
            ? await _userManager.GenerateChangeEmailTokenAsync(user, email)
            : await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var userId = await _userManager.GetUserIdAsync(user);
        var routeValues = new RouteValueDictionary()
        {
            ["userId"] = userId,
            ["code"] = code,
        };

        if (isChange)
        {
            // This is validated by the /confirmEmail endpoint on change.
            routeValues.Add("changedEmail", email);
        }

        var confirmEmailUrl = linkGenerator.GetUriByName(HttpContext, "confirmEmail", routeValues)
            ?? throw new NotSupportedException($"Could not find endpoint named confirmEmail.");

        await _emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
    }

    private static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription) =>
        TypedResults.ValidationProblem(new Dictionary<string, string[]> {
            { errorCode, [errorDescription] }
        });

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }

    private static async Task<InfoResponse> CreateInfoResponseAsync<TUser>(TUser user, UserManager<TUser> userManager)
        where TUser : class
    {
        return new()
        {
            Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
        };
    }
}
