using FluentValidation;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;

namespace SportWearShop.BusinessLogics.Validators.Products;


public class CreateProductRequestModelValidator
    : AbstractValidator<CreateProductRequestModel>
{
    private const int MaxProductNameLength = 200;
    private const int MaxDescriptionLength = 2000;
    private const int MaxBaseMaterialLength = 200;
    private const int MaxSlugLength = 200;

    public CreateProductRequestModelValidator()
    {
        RuleFor(request => request.BrandId)
            .GreaterThan(0)
            .WithMessage("Brand id must be greater than 0.");

        RuleFor(request => request.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category id must be greater than 0.");

        RuleFor(request => request.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(MaxProductNameLength)
            .WithMessage($"Product name must not exceed {MaxProductNameLength} characters.");

        RuleFor(request => request.Description)
            .MaximumLength(MaxDescriptionLength)
            .When(request => !string.IsNullOrWhiteSpace(request.Description))
            .WithMessage($"Description must not exceed {MaxDescriptionLength} characters.");

        RuleFor(request => request.BaseMaterial)
            .MaximumLength(MaxBaseMaterialLength)
            .When(request => !string.IsNullOrWhiteSpace(request.BaseMaterial))
            .WithMessage($"Base material must not exceed {MaxBaseMaterialLength} characters.");

        RuleFor(request => request.Gender)
            .IsInEnum()
            .WithMessage("Invalid product gender.");

        RuleFor(request => request.Slug)
            .MaximumLength(MaxSlugLength)
            .When(request => !string.IsNullOrWhiteSpace(request.Slug))
            .WithMessage($"Slug must not exceed {MaxSlugLength} characters.");

        RuleFor(request => request.Slug)
            .Matches(@"^[a-z0-9-]+$")
            .When(request => !string.IsNullOrWhiteSpace(request.Slug))
            .WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.");

        RuleFor(request => request.ProductName)
            .Must(name => !string.IsNullOrWhiteSpace(name) && name.Trim().Length > 0)
            .WithMessage("Product name cannot be whitespace only.");
    }
}