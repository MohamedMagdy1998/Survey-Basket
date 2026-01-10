using SurveyBasketAPI.DTOs;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IAuthService
{
    public Task<AuthResponse?> GetTokenAsync(string email, string password,CancellationToken cancellationToken = default);
}
