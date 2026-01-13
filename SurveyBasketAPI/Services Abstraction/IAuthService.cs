using SurveyBasketAPI.DTOs;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IAuthService
{
    public Task<AuthResponse?> GetTokenAsync(string email, string password,CancellationToken cancellationToken = default);
    public Task<AuthResponse?> GetNewTokenAndRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    public Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

}
