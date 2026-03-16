using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SurveyBasketAPI.DTOs.Authentication;
using SurveyBasketAPI.Entities;
using SurveyBasketAPI.Helpers;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Result_Pattern.Entities_Errors;
using SurveyBasketAPI.Services_Abstraction;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasketAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> SignInManager;
    private readonly IJwtProvider JwtProvider;
    private readonly int refreshTokenExpiryDate = 14;
    private readonly ILogger<AuthService> _logger;
    private readonly IHttpContextAccessor HttpContextAccessor;
    private readonly IEmailSender EmailSender;

    public AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtProvider jwtProvider,
        ILogger<AuthService> logger,
        IHttpContextAccessor httpContextAccessor,
        IEmailSender emailSender
        )
    {
        _userManager = userManager;
        SignInManager = signInManager;
        JwtProvider = jwtProvider;
        _logger = logger;
        HttpContextAccessor = httpContextAccessor;
        EmailSender = emailSender;
    }


    public async Task<Result> RegisterAsync(DTOs.Authentication.RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailIsExists)
            return Result.Failure(UserErrors.DuplicateEmail);

        var user = request.Adapt<ApplicationUser>();
        user.UserName = request.Email;

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {  
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code: {code}", code);

            BackgroundJob.Enqueue(() => SendConfirmationEmail(user, code));


            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }


    public async Task<Result> ResendConfirmationEmailAsync(ResendConfigurationEmail request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation code: {code}", code);


        BackgroundJob.Enqueue(() => SendConfirmationEmail(user, code));

        return Result.Success();
    }

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        // Check User
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return ((UserErrors.InvalidCredentials));
        }

        var result = await SignInManager.CheckPasswordSignInAsync(user, password, false);
        if (result.Succeeded)
        {
            // Generate Token
            var (token, expiresIn) = JwtProvider.GenerateToken(user);

            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(refreshTokenExpiryDate);


            user.RefreshTokens.Add(new RefreshTokens
            {
                Token = refreshToken!,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, token, expiresIn, refreshToken, refreshTokenExpiration);

            return (response);
        }


            return ((result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials));

        
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = JwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidCredentials);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidCredentials);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidCredentials);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result<AuthResponse>> GetNewTokenAndRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = JwtProvider.ValidateToken(token);
        if (userId == null)
        {
            return (UserErrors.InvalidCredentials);

        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (UserErrors.InvalidCredentials);
        }
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken == null)
        {
            return (UserErrors.InvalidCredentials);
        }
        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newToken, newExpiresIn) = JwtProvider.GenerateToken(user);

        var newRefreshToken = GenerateRefreshToken();

        var newRefreshTokenExpiration = DateTime.UtcNow.AddDays(refreshTokenExpiryDate);

        user.RefreshTokens.Add(new RefreshTokens
        {
            Token = newRefreshToken!,
            ExpiresOn = newRefreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, newToken, newExpiresIn, newRefreshToken!, newRefreshTokenExpiration);
       
        return (response);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }


    public async Task SendConfirmationEmail(ApplicationUser user, string code)
    {
        var origin = HttpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/emailConfirmation?userId={user.Id}&code={code}" }
            }
        );

        await EmailSender.SendEmailAsync(user.Email!, "✅ Survey Basket: Email Confirmation", emailBody);
    }


}
