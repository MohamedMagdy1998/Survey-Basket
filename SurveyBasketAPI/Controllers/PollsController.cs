using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        return  Ok(polls);
    }

    [HttpGet("{id}")] 
    public IActionResult GetById(int id)
    {
         var poll = PollService.GetById(id);

        return poll is null ? NotFound() : Ok(poll);

    }

    [HttpPost]
    public IActionResult CreatePoll(Poll poll)
    {
      
       var newPoll = PollService.AddPoll(poll);
       return CreatedAtAction(nameof(GetById),new {id = newPoll.Id},newPoll);
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePoll(int id,Poll poll)
    {
        var IsUpdated = PollService.Update(id,poll);
        
        if(!IsUpdated)  return NotFound();
        return NoContent();


    }

    [HttpDelete("{id}")]
    public IActionResult DeletePoll(int id)
    {
        var poll = PollService.Delete(id);

        if(!poll) return NotFound();

         return NoContent();

    }





}
