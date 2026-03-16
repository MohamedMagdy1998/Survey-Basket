using Azure.Core;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SurveyBasketAPI.DTOs.Polls;
using SurveyBasketAPI.Models;
using SurveyBasketAPI.Persistence;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Result_Pattern.Entities_Errors;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Services;

public class PollService(SurveyBasketDbContext context, INotificationService notificationService) : IPollService
{
    private readonly SurveyBasketDbContext _context = context;
    private readonly INotificationService _notificationService = notificationService;   

    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        
            await _context.Polls.AsNoTracking()
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);
      

    public async Task<IEnumerable<PollResponse>> GetCurentAsync(CancellationToken cancellationToken = default) =>

        await _context.Polls.AsNoTracking()
            .Where(x=>x.IsPublished && x.StartsAt<=DateOnly.FromDateTime(DateTime.UtcNow)&& x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
       .ProjectToType<PollResponse>()
       .ToListAsync(cancellationToken);
    

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
      var response =  await _context.Polls.FindAsync(id, cancellationToken);
        if (response is null)
            return (PollErrors.PollNotFound);

        return (response.Adapt<PollResponse>());

    }
       

    public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await _context.Polls.AnyAsync(p => p.Title.ToLower() == request.Title.ToLower(), cancellationToken);

        if (exists)
            return Result.Failure<PollResponse>(PollErrors.PollAlreadyExists);

        var poll = request.Adapt<Poll>();
        

        await _context.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default)
    {

        var currentPoll = await _context.Polls.FindAsync(id, cancellationToken);

        if (currentPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

         var exists = await _context.Polls.AnyAsync(p => p.Title.ToLower() == poll.Title.ToLower() && p.Id != id, cancellationToken);
        
        if (exists)
            return Result.Failure(PollErrors.PollAlreadyExists);

        currentPoll.Title = poll.Title;
        currentPoll.Summary = poll.Summary;
        currentPoll.StartsAt = poll.StartsAt;
        currentPoll.EndsAt = poll.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        _context.Remove(poll);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);
        

        poll.IsPublished = !poll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        if(poll.IsPublished&& (poll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow)))
        {
            BackgroundJob.Enqueue(() => _notificationService.SendNewPollsNotification(poll.Id));

        }

        return Result.Success();
    }

   
}