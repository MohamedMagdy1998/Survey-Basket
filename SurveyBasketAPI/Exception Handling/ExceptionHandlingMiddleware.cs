
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasketAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate Next;
    private readonly ILogger<ExceptionHandlingMiddleware> Logger;

    public ExceptionHandlingMiddleware(RequestDelegate next,ILogger<ExceptionHandlingMiddleware> logger)
    {
        Next = next;
        Logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await Next(context);

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Something went wrong {Message}", ex.Message);
            
            //var problem = Results.Problem(statusCode: StatusCodes.Status500InternalServerError);

            //var problemDetails = problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;

            var problemDetails = new ProblemDetails
            {
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred!",
                Detail = ex.Message
            };
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
