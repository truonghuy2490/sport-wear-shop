using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using SportWearShop.WebAPI.Utilities.Errors;

namespace SportWearShop.WebAPI.Utilities.Extensions;

public static class ErrorOrExtensions
{
    public static IActionResult ToProblem(
        this List<Error> errors,
        HttpContext? context = null)
    {
        var problem = ProblemDetailsMapper.FromErrors(errors, context);

        return new ObjectResult(problem)
        {
            StatusCode = problem.Status,
            ContentTypes = { "application/problem+json" }
        };
    }
}
