namespace SurveyBasketAPI.DTOs.Authentication;

public record UserLoginRequest(
    string Email,
    string Password
);

