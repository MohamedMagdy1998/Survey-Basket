namespace SurveyBasketAPI.DTOs.Results;

public record VoteResponse(
    string VoterName,
    DateTime VoteDate,
    IEnumerable<QuestionAnswerResponse> SelectedAnswers
);