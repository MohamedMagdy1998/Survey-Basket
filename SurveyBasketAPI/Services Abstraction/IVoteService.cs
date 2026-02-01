using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Result_Pattern;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IVoteService
{
    Task<Result> AddAsync(int pollId,string userId, VoteRequest request,CancellationToken cancellationToken=default);
}
