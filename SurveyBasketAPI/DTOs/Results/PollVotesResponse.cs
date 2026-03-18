namespace SurveyBasketAPI.DTOs.Results;

public record PollVotesResponse(
    string Title,
    IEnumerable<VoteResponse> Votes
);