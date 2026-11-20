using SportWearShop.Domain.Entities;
using SportWearShop.Domain.Enums;

namespace SportWearShop.Persistence.Builders;

internal sealed class ProductVariantBuilder(
    long productId,
    string sku,
    string colorCode,
    string colorName,
    string sizeCode,
    string sizeLabel,
    decimal listPrice)
{
    private decimal? _salePrice;
    private int? _weightGrams;
    private ProductVariantStatus _status = ProductVariantStatus.Active;

    public ProductVariantBuilder WithSalePrice(decimal salePrice)
    {
        _salePrice = salePrice;
        return this;
    }

    public ProductVariantBuilder WithWeightGrams(int weightGrams)
    {
        _weightGrams = weightGrams;
        return this;
    }

    public ProductVariantBuilder WithStatus(ProductVariantStatus status)
    {
        _status = status;
        return this;
    }

    public ProductVariant Build() => new()
    {
        ProductId = productId,
        Sku = sku,
        ColorCode = colorCode,
        ColorName = colorName,
        SizeCode = sizeCode,
        SizeLabel = sizeLabel,
        ListPrice = listPrice,
        SalePrice = _salePrice,
        WeightGrams = _weightGrams,
        Status = _status switch
        {
            ProductVariantStatus.Active => "ACTIVE",
            ProductVariantStatus.Inactive => "INACTIVE",
            _ => throw new ArgumentOutOfRangeException(nameof(_status), _status, "Unknown catalog status.")
        }
    };
}
