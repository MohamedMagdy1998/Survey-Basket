namespace SurveyBasketAPI.DTOs.Results;

public record VotesPerQuestionResponse(
    string Question,
    IEnumerable<VotesPerAnswerResponse> SelectedAnswers
);