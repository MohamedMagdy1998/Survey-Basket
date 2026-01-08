using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Models;

namespace SurveyBasketAPI.Mapping;

public static class ContractMapping
{
    public static PollResponse MapToPollResponse(this Poll poll)
    {
        return new PollResponse(
        poll.Id,
        poll.Title,
        poll.Description
    );
    }

    public static Poll MapToPoll(this CreatePollRequest poll)
    {
        return new()
        {
            Title = poll.Title,
            Description = poll.Description,
        };

    }

    public static IEnumerable<PollResponse> MapToPollResponse(this IEnumerable<Poll> polls)
    {
        return polls.Select(x=>x.MapToPollResponse());
    }


}
