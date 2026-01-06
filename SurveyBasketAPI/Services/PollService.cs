using Microsoft.AspNetCore.Http.HttpResults;
using SurveyBasketAPI.Models;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Services;

public class PollService : IPollService
{
    private readonly List<Poll> _polls =
       [
       new Poll(){Id=1,Title="Facebook",Description="Comment"}
    ];

   

    public IEnumerable<Poll> GetAll() => _polls;



    public Poll? GetById(int id) => _polls.FirstOrDefault(x => x.Id == id);

    public Poll AddPoll(Poll poll)
    {
        poll.Id = _polls.Count + 1;
        _polls.Add(poll);
        return poll;
    }

    public bool Update(int id, Poll poll)
    {
        var currentPoll = GetById(id);
        if (currentPoll != null)
        {
            poll.Title = currentPoll.Title;
            poll.Description = currentPoll.Description;
             return true;
        }
        return false;

    }

    public bool Delete(int id)
    {
        var TargetPoll = GetById(id);
        if (TargetPoll != null)
        {
            _polls.Remove(TargetPoll!);
                     return true;
        }
        
        return false;
    }
}
