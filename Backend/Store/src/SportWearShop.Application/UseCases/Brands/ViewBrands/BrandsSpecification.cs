using System.Linq.Expressions;
using Ardalis.Specification;
using SportWearShop.Application.Utilities;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Application.UseCases.Brands.ViewBrands;

internal sealed class BrandsSpecification : Specification<Brand, Response>
{
    internal static IReadOnlyDictionary<string, Expression<Func<Brand, object?>>> SortColumns { get; }
        = new Dictionary<string, Expression<Func<Brand, object?>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = brand => brand.BrandName,
            ["code"] = brand => brand.BrandCode,
            ["createdAtUtc"] = brand => brand.CreatedAtUtc
        };

    public BrandsSpecification(Request request, int pageNumber, int pageSize)
    {
        Query.AsNoTracking();

        if (request.IsActive is { } active)
        {
            Query.Where(brand => brand.IsActive == active);
        }

        if (request.SearchTerm is { Length: > 0 } term)
        {
            Query.Where(brand => brand.BrandName.Contains(term) || brand.BrandCode.Contains(term));
        }

        Query.ApplySorting(SortColumns[request.SortBy ?? "name"], request.SortDesc.GetValueOrDefault())
            .ThenBy(brand => brand.BrandId);
        Query.ApplyPaging(pageNumber, pageSize);
        Query.Select(brand => new Response(brand.BrandId, brand.BrandCode, brand.BrandName, brand.IsActive));
    }
}
