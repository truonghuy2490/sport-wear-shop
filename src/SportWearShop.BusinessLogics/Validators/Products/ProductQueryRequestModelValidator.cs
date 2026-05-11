using FluentValidation;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;

namespace SportWearShop.BusinessLogics.Validators.Products;

public class ProductQueryRequestModelValidator 
    : AbstractValidator<ProductQueryRequestModel>
{
    private const int MaxPageSize = 100;
    private const int MaxSearchTermLength = 100;

    public ProductQueryRequestModelValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, MaxPageSize)
            .WithMessage($"Page size must be between 1 and {MaxPageSize}.");

        RuleFor(request => request.SearchTerm)
            .MaximumLength(MaxSearchTermLength)
            .WithMessage($"Search term must not exceed {MaxSearchTermLength} characters.")
            .When(request => !string.IsNullOrWhiteSpace(request.SearchTerm));

        RuleForEach(request => request.BrandIds)
            .GreaterThan(0)
            .WithMessage("Brand id must be greater than 0.");

        RuleFor(request => request.BrandIds)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Brand ids must not contain duplicate values.");

        RuleForEach(request => request.CategoryIds)
            .GreaterThan(0)
            .WithMessage("Category id must be greater than 0.");

        RuleFor(request => request.CategoryIds)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Category ids must not contain duplicate values.");

        RuleFor(request => request.Gender)
            .IsInEnum()
            .When(request => request.Gender.HasValue)
            .WithMessage("Invalid product gender.");

        RuleFor(request => request.Status)
            .IsInEnum()
            .When(request => request.Status.HasValue)
            .WithMessage("Invalid product status.");

        RuleFor(request => request.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(request => request.MinPrice.HasValue)
            .WithMessage("Minimum price must be greater than or equal to 0.");

        RuleFor(request => request.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(request => request.MaxPrice.HasValue)
            .WithMessage("Maximum price must be greater than or equal to 0.");

        RuleFor(request => request)
            .Must(request =>
                !request.MinPrice.HasValue ||
                !request.MaxPrice.HasValue ||
                request.MinPrice.Value <= request.MaxPrice.Value)
            .WithMessage("Minimum price must be less than or equal to maximum price.");

        RuleFor(request => request.CreatedToUtc)
            .GreaterThanOrEqualTo(request => request.CreatedFromUtc)
            .When(request => request.CreatedFromUtc.HasValue && request.CreatedToUtc.HasValue)
            .WithMessage("Created to date must be greater than or equal to created from date.");

        RuleFor(request => request.SortBy)
            .IsInEnum()
            .WithMessage("Invalid product sort option.");
    }
}