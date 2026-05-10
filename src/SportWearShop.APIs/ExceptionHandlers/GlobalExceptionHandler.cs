using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.BusinessLogics.Exceptions;
using SportWearShop.Shared.ViewModels.ErrorResponseModels;
using System.Net;
using System.Text.Json;

namespace SportWearShop.APIs.ExceptionHandlers;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        RequestDelegate next,
        ILogger<GlobalExceptionHandler> logger)
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

        var errorResponse = new ErrorResponseModel();
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

            case UnauthorizedException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Title = "Unauthorized";
                errorResponse.Message = exception.Message;
                _logger.LogWarning(exception, "Unauthorized");
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
