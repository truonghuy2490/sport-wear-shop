using ErrorOr;
using FluentValidation.Results;

namespace SportWearShop.Application.UseCases.Brands.ViewBrands;

internal static class Errors
{
    internal static List<Error> Validation(IEnumerable<ValidationFailure> failures)
        => failures
            .Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage))
            .ToList();
}
