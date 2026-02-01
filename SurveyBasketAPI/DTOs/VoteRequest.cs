namespace SurveyBasketAPI.DTOs;

public record VoteRequest(IEnumerable<VoteAnswerRequest> Answers);
