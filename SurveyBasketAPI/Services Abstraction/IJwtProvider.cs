using SurveyBasketAPI.Entities;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser user);

    string? ValidateToken(string  token); // returns user id if valid to extract claims from it, otherwise returns null
}
