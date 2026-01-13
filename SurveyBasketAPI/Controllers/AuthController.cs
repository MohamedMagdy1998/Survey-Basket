using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketAPI.DTOs;
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
        if (authResult == null)
        {
            return BadRequest("Invalid email or password");
        }
        return Ok(authResult);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        var authResult = await AuthService.GetNewTokenAndRefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken, cancellationToken);
        if (authResult == null)
        {
            return BadRequest("Invalid token");
        }
        return Ok(authResult);
    }

    [HttpPut("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var isRevoked = await AuthService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return isRevoked ? Ok() : BadRequest("Operation failed");
    }

}
     
