using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.APIs.ExceptionHandlers.SportWearShop.APIs.Middlewares;
using SportWearShop.BusinessLogics.Exceptions;
using System.Net;
using System.Text.Json;

namespace SportWearShop.APIs.ExceptionHandlers;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();
        int statusCode;

        switch (exception)
        {
            case BadRequestException:
                statusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Title = "Bad Request";
                errorResponse.Message = exception.Message;
                _logger.LogWarning(exception, "Bad request");
                break;

            case NotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Title = "Not Found";
                errorResponse.Message = exception.Message;
                _logger.LogInformation(exception, "Resource not found");
                break;

            case ConflictException:
                statusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Title = "Conflict";
                errorResponse.Message = exception.Message;
                _logger.LogWarning(exception, "Conflict error");
                break;

            case ForbiddenException:
                statusCode = (int)HttpStatusCode.Forbidden;
                errorResponse.Title = "Forbidden";
                errorResponse.Message = exception.Message;
                _logger.LogWarning(exception, "Forbidden access");
                break;

            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Title = "Unauthorized";
                errorResponse.Message = "You are not authorized";
                _logger.LogWarning(exception, "Unauthorized access");
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Title = "Internal Server Error";
                errorResponse.Message = "An unexpected error occurred. Please try again later.";
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        response.StatusCode = statusCode;
        errorResponse.Status = statusCode;
        errorResponse.TraceId = context.TraceIdentifier;
        errorResponse.Timestamp = DateTime.UtcNow;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await response.WriteAsync(jsonResponse);
    }
}
public class ErrorResponse
{
    public string? Title { get; set; }
    public int Status { get; set; }
    public string? Message { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; }
}