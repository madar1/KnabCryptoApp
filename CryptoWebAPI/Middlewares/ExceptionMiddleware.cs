using System.Net;
using System.Text.Json;
using CryptoWebAPI.Models;

namespace CryptoWebAPI.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, $"An error occurred: {ex.Message}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorDetails
        {
            StatusCode = (int)HttpStatusCode.InternalServerError,
            Message = "An unexpected error occurred. Please try again later.",
            ExceptionMessage = exception.Message
        };

        response.StatusCode = errorResponse.StatusCode;
        var json = JsonSerializer.Serialize(errorResponse);
        return response.WriteAsync(json);
    }
}