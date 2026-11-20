using System.Diagnostics;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace SportWearShop.WebAPI.Utilities.Errors;

public static class ProblemDetailsMapper
{
    public static ProblemDetails FromErrors(
        IReadOnlyList<Error> errors,
        HttpContext? context = null)
    {
        if (errors.Count == 0)
        {
            return FromStatusCode(
                StatusCodes.Status500InternalServerError, context);
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            var validationErrors = errors
                .GroupBy(error => error.Code)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.Description).ToArray());

            var validationProblem = new ValidationProblemDetails(validationErrors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            };

            SetRequestDetails(validationProblem, context);
            return validationProblem;
        }

        // For mixed errors, use the first non-validation error for the response.
        var primaryError = errors.First(error => error.Type != ErrorType.Validation);

        var statusCode = primaryError.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = FromStatusCode(statusCode, context);

        if (statusCode < StatusCodes.Status500InternalServerError)
        {
            problem.Detail = primaryError.Description;
            problem.Extensions["code"] = primaryError.Code;
        }

        return problem;
    }

    public static ProblemDetails FromStatusCode(
        int statusCode,
        HttpContext? context = null)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = ReasonPhrases.GetReasonPhrase(statusCode),
            Detail = statusCode >= StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred while processing the request."
                : null
        };

        SetRequestDetails(problem, context);
        return problem;
    }

    private static void SetRequestDetails(ProblemDetails problem, HttpContext? context)
    {
        if (context is null)
        {
            return;
        }

        problem.Instance = context.Request.Path.Value;
        problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;
    }
}
