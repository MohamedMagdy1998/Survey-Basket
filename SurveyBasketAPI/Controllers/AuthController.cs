using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        var authResult = await AuthService.GetTokenAsync(loginRequest.Email, loginRequest.Password, cancellationToken);
        if (authResult == null)
        {
            return BadRequest("Invalid email or password");
        }
        return Ok(authResult);
    }

}
