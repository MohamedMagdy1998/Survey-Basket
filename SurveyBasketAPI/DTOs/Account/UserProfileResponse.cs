namespace SurveyBasketAPI.DTOs.Account;

public record UserProfileResponse
    (
    string UserName,
    string Email,
    string FirstName,
    string LastName
    );