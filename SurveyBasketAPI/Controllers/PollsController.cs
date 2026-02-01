using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketAPI.DTOs.Polls;
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
        var response = await _pollService.GetAllAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var response = await _pollService.GetCurentAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);

        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest request,
        CancellationToken cancellationToken)
    {
        var newPoll = await _pollService.AddAsync(request, cancellationToken);

        if (newPoll.IsFailure)
            return newPoll.ToProblem();
        
        return CreatedAtAction(nameof(Get), new { id = newPoll.Value.Id }, newPoll.Adapt<PollResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request,
        CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.UpdateAsync(id, request, cancellationToken);

        return (isUpdated.IsSuccess) ? NoContent() : isUpdated.ToProblem();
       
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _pollService.DeleteAsync(id, cancellationToken);

        return (isDeleted.IsFailure) ? NoContent() : isDeleted.ToProblem();
    }

    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        return (isUpdated.IsFailure) ? isUpdated.ToProblem() : NoContent();

    }
}
