using Ardalis.Specification;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Application.Features.Brands.GetBrands;

internal sealed class GetBrandsSpecification : Specification<Brand, BrandDto>
{
    public GetBrandsSpecification(bool? isActive)
    {
        if (isActive is { } active)
        {
            Query.Where(brand => brand.IsActive == active);
        }

        Query.AsNoTracking()
            .OrderBy(brand => brand.BrandName)
            .ThenBy(brand => brand.BrandId);

        Query.Select(brand => new BrandDto(
            brand.BrandId, brand.BrandCode, brand.BrandName, brand.IsActive));
    }
}
