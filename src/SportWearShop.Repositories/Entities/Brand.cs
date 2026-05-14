using System;
using System.Collections.Generic;

namespace SportWearShop.Repositories.Entities;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string BrandCode { get; set; } = null!;
    public string? BrandImage { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
