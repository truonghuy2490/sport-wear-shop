using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Builders;

internal sealed class CategoryBuilder(string code, string name)
{
    private int? _parentCategoryId;
    private string? _description;
    private int _sortOrder;
    private bool _isActive = true;

    public CategoryBuilder WithParent(int parentCategoryId)
    {
        _parentCategoryId = parentCategoryId;
        return this;
    }

    public CategoryBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public CategoryBuilder WithSortOrder(int sortOrder)
    {
        _sortOrder = sortOrder;
        return this;
    }

    public CategoryBuilder WithActiveStatus(bool active)
    {
        _isActive = active;
        return this;
    }

    public Category Build() => new()
    {
        CategoryCode = code,
        CategoryName = name,
        ParentCategoryId = _parentCategoryId,
        Description = _description,
        SortOrder = _sortOrder,
        IsActive = _isActive
    };
}
