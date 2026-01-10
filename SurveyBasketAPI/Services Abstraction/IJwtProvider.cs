using SurveyBasketAPI.Entities;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser user);
}
