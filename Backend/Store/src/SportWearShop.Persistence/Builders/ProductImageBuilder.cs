using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Builders;

internal sealed class ProductImageBuilder(long productId, string imageUrl)
{
    private long? _productVariantId;
    private string? _altText;
    private int _sortOrder;
    private bool _isPrimary;

    public ProductImageBuilder ForVariant(long productVariantId)
    {
        _productVariantId = productVariantId;
        return this;
    }

    public ProductImageBuilder WithAltText(string altText)
    {
        _altText = altText;
        return this;
    }

    public ProductImageBuilder WithSortOrder(int sortOrder)
    {
        _sortOrder = sortOrder;
        return this;
    }

    public ProductImageBuilder AsPrimary()
    {
        _isPrimary = true;
        return this;
    }

    public ProductImage Build() => new()
    {
        ProductId = productId,
        ProductVariantId = _productVariantId,
        ImageUrl = imageUrl,
        AltText = _altText,
        SortOrder = _sortOrder,
        IsPrimary = _isPrimary
    };
}
