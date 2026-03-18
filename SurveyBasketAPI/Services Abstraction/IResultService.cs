using SurveyBasketAPI.DTOs.Results;
using SurveyBasketAPI.Result_Pattern;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IResultService
{
    Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default);
}
