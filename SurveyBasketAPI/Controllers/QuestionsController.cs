using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasketAPI.DTOs;
using SurveyBasketAPI.Result_Pattern;
using SurveyBasketAPI.Result_Pattern.Entities_Errors;
using SurveyBasketAPI.Services_Abstraction;

namespace SurveyBasketAPI.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetQuestions([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var questionsResult = await _questionService.GetAllAsync(pollId, cancellationToken);
        return (questionsResult.IsSuccess) ? Ok(questionsResult.Value) : questionsResult.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute]int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {

        var createdQuestion = await _questionService.AddAsync(pollId, request, cancellationToken);

        return (createdQuestion.IsSuccess) ? CreatedAtAction(nameof(GetQuestions), new { pollId, createdQuestion.Value.Id }, createdQuestion.Value)
                                                : createdQuestion.ToProblem();
    }

    [HttpGet("{questionId}")]
    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int questionId, CancellationToken cancellationToken)
    {
        var questionResult = await _questionService.GetAsync(pollId, questionId, cancellationToken);
        return (questionResult.IsSuccess) ? Ok(questionResult.Value) : questionResult.ToProblem();
        
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute] int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _questionService.UpdateAsync(pollId, id, request, cancellationToken);

        return (result.IsSuccess) ? NoContent() : result.ToProblem();
               
    }

    [HttpPut("{questionId}/toggleStatus")]
    public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int questionId, CancellationToken cancellationToken)
    {
        var result = await _questionService.ToggleStatusAsync(pollId, questionId, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

}