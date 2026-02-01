namespace SurveyBasketAPI.DTOs.Results;

public record VotesPerDayResponse(
    DateOnly Date,
    int NumberOfVotes
);