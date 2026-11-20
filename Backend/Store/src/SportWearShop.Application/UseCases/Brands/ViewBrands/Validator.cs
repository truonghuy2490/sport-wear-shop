using FluentValidation;
using SportWearShop.Application.Common.Validators;

namespace SportWearShop.Application.UseCases.Brands.ViewBrands;

internal sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(request => request.PageNumber)
            .MustBeValidPageNumber()
            .Must((request, pageNumber) =>
                ((long)pageNumber.GetValueOrDefault(1) - 1) *
                request.PageSize.GetValueOrDefault(10) <= int.MaxValue)
            .WithMessage("The requested page offset is too large.");

        RuleFor(request => request.PageSize).MustBeValidPageSize();

        RuleFor(request => request.SearchTerm)
            .MaximumLength(100);

        RuleFor(request => request.SortBy)
            .Must(value => value is null || BrandsSpecification.SortColumns.ContainsKey(value))
            .WithMessage("Sort by must be name, code, or createdAtUtc.");
    }
}
