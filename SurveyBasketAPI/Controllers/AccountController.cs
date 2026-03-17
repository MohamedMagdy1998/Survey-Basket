using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketAPI.DTOs.Account;
using SurveyBasketAPI.Extensions;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Controllers;

[Route("AccountInfo")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IUserService UserService;

    public AccountController(IUserService userService)
    {
        UserService = userService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetUserProfile()
    {
        var user = await UserService.GetUserProfileAsync(UserExtensions.GetUserId(User)!);

        return Ok(user.Value);
    }

    [HttpPut("")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileRequest request)
    {
        await UserService.UpdateUserProfileAsync(UserExtensions.GetUserId(User)!, request);
        
        return NoContent();
    }

    [HttpPut("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await UserService.ChangePasswordAsync(UserExtensions.GetUserId(User)!, request);
        if (result.IsFailure)
        {
            return result.ToProblem();
        }

        return NoContent();
    }

}
