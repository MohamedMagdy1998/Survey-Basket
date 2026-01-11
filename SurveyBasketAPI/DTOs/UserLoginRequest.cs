namespace SurveyBasketAPI.DTOs;

public record UserLoginRequest(
    string Email,
    string Password
);

