using SportWearShop.Domain.Entities;
using SportWearShop.Domain.Enums;

namespace SportWearShop.Persistence.Builders;

internal sealed class ProductBuilder(int brandId, int categoryId, string code, string name, string slug)
{
    private string? _description;
    private string? _gender;
    private string? _baseMaterial;
    private ProductStatus _status = ProductStatus.Active;

    public ProductBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ProductBuilder WithGender(string gender)
    {
        _gender = gender;
        return this;
    }

    public ProductBuilder WithBaseMaterial(string baseMaterial)
    {
        _baseMaterial = baseMaterial;
        return this;
    }

    public ProductBuilder WithStatus(ProductStatus status)
    {
        _status = status;
        return this;
    }

    public Product Build() => new()
    {
        BrandId = brandId,
        CategoryId = categoryId,
        ProductCode = code,
        ProductName = name,
        Slug = slug,
        Description = _description,
        Gender = _gender,
        BaseMaterial = _baseMaterial,
        Status = _status switch
        {
            ProductStatus.Draft => "DRAFT",
            ProductStatus.Active => "ACTIVE",
            ProductStatus.Inactive => "INACTIVE",
            ProductStatus.Discontinued => "DISCONTINUED",
            _ => throw new ArgumentOutOfRangeException(nameof(_status), _status, "Unknown catalog status.")
        }
    };
}
