using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using SurveyBasketAPI.Common;
using SurveyBasketAPI.DTOs.Answers;
using SurveyBasketAPI.DTOs.Questions;
using SurveyBasketAPI.Entities;
using SurveyBasketAPI.Persistence;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Result_Pattern.Entities_Errors;
using SurveyBasketAPI.Services_Abstraction;
using System.Buffers;
using System.Diagnostics.Contracts;

namespace SurveyBasketAPI.Services;

public class QuestionService : IQuestionService
{
    private readonly SurveyBasketDbContext _dbContext;
    private const string _cachePrefix = "availableQuestions";
    private readonly HybridCache _hybridCache;
    private readonly ILogger<QuestionService> _logger;

    public QuestionService(SurveyBasketDbContext dbContext, HybridCache hybridCache, ILogger<QuestionService> logger)
    { 
        _dbContext = dbContext;
        _hybridCache = hybridCache;
        _logger = logger;
    }
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken)
    {
        var isPollExists = await _dbContext.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
       
        if (!isPollExists)
        {
            return  Result.Failure<QuestionResponse>(PollErrors.PollNotFound);
        }

        var isQuestionExists = await _dbContext.Questions
            .AnyAsync(q => q.Content == request.Content && q.PollId == pollId, cancellationToken);

        if (isQuestionExists)
        {
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionAlreadyExists);
        }

        var question = request.Adapt<QuestionRequest,Question>();
       
        question.PollId = pollId;
      

        await _dbContext.Questions.AddAsync(question, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

        return Result.Success(question.Adapt<Question,QuestionResponse>());
    }

    public async Task<Result<PaginatedResult<QuestionResponse>>> GetAllAsync(int pollId, PaginationFilters paginationFilters, CancellationToken cancellationToken)
    {
        var isPollExists = await _dbContext.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!isPollExists)
        {
            return Result.Failure<PaginatedResult<QuestionResponse>>(PollErrors.PollNotFound);
        }

        var questions = _dbContext.Questions
            .Where(q => q.PollId == pollId&&(string.IsNullOrWhiteSpace(paginationFilters.SearchValue)|| q.Content.ToLower().Contains(paginationFilters.SearchValue.ToLower())))
            .Include(q => q.Answers)
            .AsNoTracking();

        if(!string.IsNullOrWhiteSpace(paginationFilters.SortColumn) && paginationFilters.SortColumn.Equals("content", StringComparison.OrdinalIgnoreCase))
        {
            questions = paginationFilters.SortDirection!.Equals("Desc", StringComparison.OrdinalIgnoreCase) ? questions.OrderByDescending(q => q.Content) : questions.OrderBy(q => q.Content);
        }
           
        if (!questions.Any())
        {
            return Result.Failure<PaginatedResult<QuestionResponse>>(QuestionErrors.QuestionNotFound);
        }

        var paginatedResult = await PaginatedResult<QuestionResponse>.CreatePagination(questions.ProjectToType<QuestionResponse>(), paginationFilters.PageNumber, paginationFilters.PageSize, cancellationToken);
        return Result.Success(paginatedResult);
    }

    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId, CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions
             .Where(x => x.PollId == pollId && x.Id == questionId)
             .Include(x => x.Answers)
             .ProjectToType<QuestionResponse>()
             .AsNoTracking()
             .SingleOrDefaultAsync(cancellationToken);

        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }

    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var questionIsExists = await _dbContext.Questions
            .AnyAsync(x => x.PollId == pollId
                && x.Id != id
                && x.Content == request.Content,
                cancellationToken
            );

        if (questionIsExists)
            return Result.Failure(QuestionErrors.QuestionAlreadyExists);

        var question = await _dbContext.Questions
            .Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        //current answers
        var currentAnswers = question.Answers.Select(x => x.Content).ToList();

        //add new answer
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer =>
        {
            question.Answers.Add(new Answer { Content = answer });
        });

        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content);
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _dbContext.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var hasVote =  await _dbContext.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

        var pollIsExists = await _dbContext.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var cacheKey = $"{_cachePrefix}-{pollId}";

        var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(
            cacheKey,
            async cacheEntry => await _dbContext.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Include(x => x.Answers)
            .Select(q => new QuestionResponse(
                q.Id,
                q.Content,
                q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken));

        return Result.Success(questions);
    }


}
