using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Mapping;
using SurveyBasketAPI.Models;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Result_Pattern.Entities_Errors;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);

        var response = polls.Adapt<IEnumerable<PollResponse>>();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem(statusCode: StatusCodes.Status400BadRequest);
        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest request,
        CancellationToken cancellationToken)
    {
        var newPoll = await _pollService.AddAsync(request, cancellationToken);

        if (newPoll.IsFailure)
            return newPoll.ToProblem(statusCode: StatusCodes.Status409Conflict);
        
        return CreatedAtAction(nameof(Get), new { id = newPoll.Value.Id }, newPoll.Adapt<PollResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request,
        CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.UpdateAsync(id, request, cancellationToken);

        if (isUpdated.IsSuccess)
            return NoContent();

        return isUpdated.Error.Equals(PollErrors.PollAlreadyExists)
                 ? isUpdated.ToProblem(StatusCodes.Status409Conflict)
                 : isUpdated.ToProblem(StatusCodes.Status404NotFound);

       
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _pollService.DeleteAsync(id, cancellationToken);

        if (isDeleted.IsFailure)
            return isDeleted.ToProblem(statusCode: StatusCodes.Status404NotFound);

        return NoContent();
    }

    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        if (isUpdated.IsFailure)
            return isUpdated.ToProblem(statusCode: StatusCodes.Status404NotFound);

        return NoContent();
    }
}
