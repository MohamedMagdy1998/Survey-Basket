namespace SurveyBasketAPI.DTOs.Authentication;

public record ConfirmEmailRequest(string UserId, string Code);