namespace SurveyBasketAPI.DTOs.Results;

public record VotesPerAnswerResponse(
    string Answer,
    int Count
);
