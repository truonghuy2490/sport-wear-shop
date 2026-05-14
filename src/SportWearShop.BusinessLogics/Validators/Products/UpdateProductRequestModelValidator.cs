using FluentValidation;
using SportWearShop.BusinessLogics.ResponseModels.ProductModels;

namespace SportWearShop.BusinessLogics.Validators.Products;

public class UpdateProductRequestModelValidator
    : AbstractValidator<UpdateProductRequestModel>
{
    private const int MaxProductNameLength = 200;
    private const int MaxDescriptionLength = 2000;
    private const int MaxBaseMaterialLength = 200;
    private const int MaxSlugLength = 200;

    public UpdateProductRequestModelValidator()
    {
        RuleFor(request => request.ProductName)
            .MaximumLength(MaxProductNameLength)
            .When(request => !string.IsNullOrWhiteSpace(request.ProductName))
            .WithMessage($"Product name must not exceed {MaxProductNameLength} characters.");

        RuleFor(request => request.ProductName)
            .Must(name => name == null || !string.IsNullOrWhiteSpace(name))
            .WithMessage("Product name cannot be whitespace only.");

        RuleFor(request => request.Description)
            .MaximumLength(MaxDescriptionLength)
            .When(request => !string.IsNullOrWhiteSpace(request.Description))
            .WithMessage($"Description must not exceed {MaxDescriptionLength} characters.");

        RuleFor(request => request.BaseMaterial)
            .MaximumLength(MaxBaseMaterialLength)
            .When(request => !string.IsNullOrWhiteSpace(request.BaseMaterial))
            .WithMessage($"Base material must not exceed {MaxBaseMaterialLength} characters.");

        RuleFor(request => request.Slug)
            .MaximumLength(MaxSlugLength)
            .When(request => !string.IsNullOrWhiteSpace(request.Slug))
            .WithMessage($"Slug must not exceed {MaxSlugLength} characters.");

        RuleFor(request => request.Slug)
            .Matches(@"^[a-z0-9-]+$")
            .When(request => !string.IsNullOrWhiteSpace(request.Slug))
            .WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.");

        RuleFor(request => request.Gender)
            .IsInEnum()
            .When(request => request.Gender.HasValue)
            .WithMessage("Invalid product gender.");

        RuleFor(request => request.Status)
            .IsInEnum()
            .When(request => request.Status.HasValue)
            .WithMessage("Invalid product status.");

        RuleFor(request => request.BrandId)
            .GreaterThan(0)
            .When(request => request.BrandId.HasValue)
            .WithMessage("Brand id must be greater than 0.");

        RuleFor(request => request.CategoryId)
            .GreaterThan(0)
            .When(request => request.CategoryId.HasValue)
            .WithMessage("Category id must be greater than 0.");
    }
}