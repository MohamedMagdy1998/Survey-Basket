using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasketAPI.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger,IHostEnvironment environment) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> Logger = logger;
    private readonly IHostEnvironment Environment = environment;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        Logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };
        var problemDetails = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Status = statusCode,
            Title = title,
            Detail = Environment.IsDevelopment() ? exception.Message : "An unexpected error occurred. Please try again later."
        };
       await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;   
    }
}
