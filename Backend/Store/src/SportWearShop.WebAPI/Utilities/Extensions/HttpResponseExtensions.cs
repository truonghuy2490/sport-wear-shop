using Microsoft.AspNetCore.Mvc;

namespace SportWearShop.WebAPI.Utilities.Extensions;

public static class HttpResponseExtensions
{
    public static async ValueTask WriteProblemDetailsAsync(
        this HttpResponse response,
        ProblemDetails problem,
        IProblemDetailsService problemDetailsService,
        CancellationToken cancellationToken = default)
    {
        response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        if (!await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = response.HttpContext,
            ProblemDetails = problem
        }))
        {
            await response.WriteAsJsonAsync(
                problem,
                problem.GetType(),
                options: null,
                contentType: "application/problem+json",
                cancellationToken: cancellationToken);
        }
    }
}
