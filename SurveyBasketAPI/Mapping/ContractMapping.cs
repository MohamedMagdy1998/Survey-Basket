using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Models;
using System.Security.Cryptography;

namespace SurveyBasketAPI.Mapping;

public static class ContractMapping
{
    public static PollResponse MapToPollResponse(this Poll poll)
    {
        return new PollResponse(
        poll.Id,
        poll.Title,
        poll.Summary,
        poll.IsPublished,
        poll.StartsAt,
        poll.EndsAt
    );
    }

    public static Poll MapToPoll(this PollRequest poll)
    {
        return new()
        {
            Title = poll.Title,
            Summary = poll.Summary,
        };

    }

    public static IEnumerable<PollResponse> MapToPollResponse(this IEnumerable<Poll> polls)
    {
        return polls.Select(x=>x.MapToPollResponse());
    }


}
