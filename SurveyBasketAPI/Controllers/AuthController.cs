using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Result_Pattern.Entities_Errors;
using SurveyBasketAPI.Services;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService AuthService;

    public AuthController(IAuthService authService)
    {
        AuthService = authService;
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
     
