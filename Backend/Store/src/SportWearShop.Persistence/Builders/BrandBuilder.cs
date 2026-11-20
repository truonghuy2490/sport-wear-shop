using SportWearShop.Domain.Entities;

namespace SportWearShop.Persistence.Builders;

internal sealed class BrandBuilder(string code, string name)
{
    private bool isActive = true;

    public BrandBuilder WithActiveStatus(bool active)
    {
        isActive = active;
        return this;
    }

    public Brand Build() => new()
    {
        BrandCode = code,
        BrandName = name,
        IsActive = isActive
    };
}
