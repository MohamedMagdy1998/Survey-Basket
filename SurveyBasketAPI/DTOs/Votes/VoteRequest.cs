namespace SurveyBasketAPI.DTOs.Votes;

public record VoteRequest(IEnumerable<VoteAnswerRequest> Answers);
