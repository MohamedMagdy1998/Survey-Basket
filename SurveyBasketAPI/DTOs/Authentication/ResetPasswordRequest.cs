namespace SurveyBasketAPI.DTOs.Authentication;

public record ResetPasswordRequest(
    string Email,
    string Code,
    string NewPassword
);