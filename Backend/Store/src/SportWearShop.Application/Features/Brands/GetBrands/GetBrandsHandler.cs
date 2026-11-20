using SportWearShop.Application.Abstractions.DataAccess;
using SportWearShop.Domain.Entities;

namespace SportWearShop.Application.Features.Brands.GetBrands;

public sealed class GetBrandsHandler(IRepository<Brand, int> repository)
{
    public Task<List<BrandDto>> HandleAsync(bool? isActive, CancellationToken cancellationToken = default)
        => repository.ListAsync(new GetBrandsSpecification(isActive), cancellationToken);
}
