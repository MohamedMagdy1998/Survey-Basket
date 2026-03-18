using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Abstractions.Consts;
using SurveyBasketAPI.DTOs.Authentication;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Services;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Controllers;

[Route("[controller]")]
[ApiController]
[EnableRateLimiting(RateLimiters.IpLimiter)]
public class AuthController : ControllerBase
{
    private readonly IAuthService AuthService;

    public AuthController(IAuthService authService)
    {
        AuthService = authService;
    }
    
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await AuthService.ConfirmEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfigurationEmail request, CancellationToken cancellationToken)
    {
        var result = await AuthService.ResendConfirmationEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("register")]
    [DisableRateLimiting]

    public async Task<IActionResult> Register([FromBody] DTOs.Authentication.RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await AuthService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }


    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
    {
        var result = await AuthService.SendResetPasswordCodeAsync(request.Email);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] DTOs.Authentication.ResetPasswordRequest request)
    {
        var result = await AuthService.ResetPasswordAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }


    [HttpPost("")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        var authResult = await AuthService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);
       
        return (authResult.IsFailure)? authResult.ToProblem() : Ok(authResult.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        var authResult = await AuthService.GetNewTokenAndRefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken, cancellationToken);

        return (authResult.IsFailure) ? authResult.ToProblem() : Ok(authResult.Value);
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var isRevoked = await AuthService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return isRevoked.IsSuccess ? Ok() : isRevoked.ToProblem();
    }

}
     
