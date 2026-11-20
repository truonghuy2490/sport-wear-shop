using FluentValidation;

namespace SportWearShop.Application.Common.Validators;

internal static class ValidatorExtensions
{
    internal static IRuleBuilderOptions<T, int?> MustBeValidPageNumber<T>(
        this IRuleBuilder<T, int?> ruleBuilder)
        => ruleBuilder.GreaterThan(0)
            .WithMessage("Page number must be greater than 0.");

    internal static IRuleBuilderOptions<T, int?> MustBeValidPageSize<T>(
        this IRuleBuilder<T, int?> ruleBuilder,
        int maxPageSize = 100)
        => ruleBuilder.InclusiveBetween(1, maxPageSize)
            .WithMessage($"Page size must be between 1 and {maxPageSize}.");

    internal static IRuleBuilderOptions<T, string?> MustBeValidGuid<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder.NotEmpty()
            .WithMessage("{PropertyName} is required.")
            .Must(value => string.IsNullOrWhiteSpace(value) || Guid.TryParse(value, out _))
            .WithMessage("{PropertyName} must be a valid GUID.");
}
