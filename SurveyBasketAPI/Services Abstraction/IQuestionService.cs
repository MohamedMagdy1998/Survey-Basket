using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Result_Pattern;

namespace SurveyBasketAPI.Services_Abstraction;

public interface IQuestionService
{
    public Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request,CancellationToken cancellationToken);
    public Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync (int pollId, CancellationToken cancellationToken);
    public Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken);

    public Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest request, CancellationToken cancellationToken);

    public Task<Result> ToggleStatusAsync(int pollId, int questionId, CancellationToken cancellationToken = default);

    public Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default);


}
