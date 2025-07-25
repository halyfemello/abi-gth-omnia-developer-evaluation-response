using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace DeveloperEvaluation.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = CreateProblemDetails(context, exception);

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(json);
    }

    private static ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = context.Request.Path,
            Detail = GetErrorDetail(exception),
            Type = GetErrorType(exception)
        };

        switch (exception)
        {
            case ArgumentException:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Bad Request";
                break;

            case UnauthorizedAccessException:
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                problemDetails.Title = "Unauthorized";
                break;

            case KeyNotFoundException:
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Not Found";
                break;

            case InvalidOperationException when exception.Message.Contains("not found"):
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Not Found";
                break;

            case InvalidOperationException:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Bad Request";
                break;

            default:
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An error occurred while processing your request.";
                break;
        }

        return problemDetails;
    }

    private static string GetErrorDetail(Exception exception)
    {
        return exception switch
        {
            ArgumentException => exception.Message,
            UnauthorizedAccessException => "Access to the requested resource is unauthorized.",
            KeyNotFoundException => "The requested resource was not found.",
            InvalidOperationException => exception.Message,
            _ => "An error occurred while processing your request."
        };
    }

    private static string GetErrorType(Exception exception)
    {
        return exception switch
        {
            ArgumentException => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            UnauthorizedAccessException => "https://tools.ietf.org/html/rfc7235#section-3.1",
            KeyNotFoundException => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            InvalidOperationException when exception.Message.Contains("not found") => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            InvalidOperationException => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }
}
