using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketAPI.DTOs.Votes;
using SurveyBasketAPI.Extensions;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Result_Pattern.Entities_Errors;
using SurveyBasketAPI.Services_Abstraction;
using System.Security.Claims;

namespace SurveyBasketAPI.Controllers;

[Route("api/polls/{pollid}/vote")]
[ApiController]
[Authorize]
public class VotesController : ControllerBase
{
    private readonly IQuestionService _questionService;
    private readonly IVoteService _voteService;

    public VotesController(IQuestionService questionService, IVoteService voteService)
    {
        _questionService = questionService;
        _voteService = voteService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await _questionService.GetAvailableAsync(pollId, userId!, cancellationToken);

        return (result.IsSuccess)? Ok(result.Value):result.ToProblem();
        
            

    }

    [HttpPost("")]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? Created() : BadRequest();
    }

}
