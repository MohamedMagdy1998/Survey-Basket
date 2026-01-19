using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Result_Pattern;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IAuthService
{
    public Task<Result<AuthResponse>> GetTokenAsync(string email, string password,CancellationToken cancellationToken = default);
    public Task<Result<AuthResponse>> GetNewTokenAndRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    public Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

}
