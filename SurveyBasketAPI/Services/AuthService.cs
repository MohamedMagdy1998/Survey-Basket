using Microsoft.AspNetCore.Identity;
using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Entities;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtProvider JwtProvider;

    public AuthService(UserManager<ApplicationUser> userManager,IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        JwtProvider = jwtProvider;
    }
    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        // Check User
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        // Check Password
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordValid)
        {
            return null;
        }

        // Generate Token
        var (token, expiresIn) = JwtProvider.GenerateToken(user);

        return new AuthResponse(user.Id, user.FirstName,user.LastName,user.Email, token, expiresIn);
        

    }
}
