using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Mapping;
using SurveyBasketAPI.Models;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService PollService = pollService;

    [HttpGet("")]
    public IActionResult GetAll()
    {
        var polls = PollService.GetAll();
        return  Ok(polls.Adapt<IEnumerable<PollResponse>>());
    }

    [HttpGet("{id}")] 
    public IActionResult GetById([FromRoute] int id)
    {
         var poll = PollService.GetById(id);

        return poll is null ? NotFound() : Ok(poll.Adapt<PollResponse>());

    }

    [HttpPost("")]
    public IActionResult CreatePoll([FromBody] CreatePollRequest poll)
    {
      
       var newPoll = PollService.AddPoll(poll.Adapt<Poll>());
       return CreatedAtAction(nameof(GetById),new {id = newPoll.Id},newPoll);
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePoll([FromRoute] int id, [FromBody] CreatePollRequest poll)
    {
        var IsUpdated = PollService.Update(id,poll.Adapt<Poll>());
        
        if(!IsUpdated)  return NotFound();
        return NoContent();


    }

    [HttpDelete("{id}")]
    public IActionResult DeletePoll([FromRoute] int id)
    {
        var poll = PollService.Delete(id);

        if(!poll) return NotFound();

         return NoContent();

    }





}
