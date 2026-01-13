using Microsoft.AspNetCore.Identity;
using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Entities;
using SurveyBasketAPI.Services_Abstraction;
using System.Security.Cryptography;

namespace SurveyBasketAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtProvider JwtProvider;
    private readonly int refreshTokenExpiryDate = 14;

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

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpirattion = DateTime.UtcNow.AddDays(refreshTokenExpiryDate);


        user.RefreshTokens.Add(new RefreshTokens
        {
            Token = refreshToken!,
            ExpiresOn = refreshTokenExpirattion
        });

        await _userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.FirstName,user.LastName,user.Email, token, expiresIn, refreshToken, refreshTokenExpirattion);
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = JwtProvider.ValidateToken(token);

        if (userId is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return false;

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task<AuthResponse?> GetNewTokenAndRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = JwtProvider.ValidateToken(token);
        if (userId == null)
        {
            return null!;
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null!;
        }
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.IsActive);
        if (userRefreshToken == null)
        {
            return null!;
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
        return new AuthResponse(user.Id, user.FirstName, user.LastName, user.Email, newToken, newExpiresIn, newRefreshToken!, newRefreshTokenExpiration);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }


}
