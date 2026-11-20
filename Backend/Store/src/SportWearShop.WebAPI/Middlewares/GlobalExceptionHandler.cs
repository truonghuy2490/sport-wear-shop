using Microsoft.AspNetCore.Diagnostics;
using SportWearShop.WebAPI.Utilities.Errors;
using SportWearShop.WebAPI.Utilities.Extensions;

namespace SportWearShop.WebAPI.Middlewares;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception is BadHttpRequestException badRequest
            ? badRequest.StatusCode
            : StatusCodes.Status500InternalServerError;

        logger.Log(
            statusCode >= StatusCodes.Status500InternalServerError ? LogLevel.Error : LogLevel.Warning,
            exception,
            "Request failed with status {StatusCode}. Trace identifier: {TraceIdentifier}",
            statusCode,
            httpContext.TraceIdentifier);

        var problem = ProblemDetailsMapper.FromStatusCode(statusCode, httpContext);
        await httpContext.Response.WriteProblemDetailsAsync(problem, problemDetailsService, cancellationToken);
        return true;
    }
}
