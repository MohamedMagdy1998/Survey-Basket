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
        if (questionsResult.IsSuccess)
        {
            return Ok(questionsResult.Value);
        }
        return questionsResult.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute]int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {

        var createdQuestion = await _questionService.AddAsync(pollId, request, cancellationToken);

        if (createdQuestion.IsSuccess)
        {
            return CreatedAtAction(nameof(GetQuestions), new { pollId,createdQuestion.Value.Id }, createdQuestion.Value);
        } 

        return createdQuestion.Error.Equals(QuestionErrors.QuestionAlreadyExists)
            ? createdQuestion.ToProblem(StatusCodes.Status409Conflict)
            : createdQuestion.ToProblem(StatusCodes.Status404NotFound);

    }

    [HttpGet("{questionId}")]
    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int questionId, CancellationToken cancellationToken)
    {
        var questionResult = await _questionService.GetAsync(pollId, questionId, cancellationToken);
        if (questionResult.IsSuccess)
        {
            return Ok(questionResult.Value);
        }
        return questionResult.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute] int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
    {
        var result = await _questionService.UpdateAsync(pollId, id, request, cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        return result.Error.Equals(QuestionErrors.QuestionAlreadyExists)
                ? result.ToProblem(StatusCodes.Status409Conflict)
                : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPut("{questionId}/toggleStatus")]
    public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int questionId, CancellationToken cancellationToken)
    {
        var result = await _questionService.ToggleStatusAsync(pollId, questionId, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem(StatusCodes.Status404NotFound);
    }

}