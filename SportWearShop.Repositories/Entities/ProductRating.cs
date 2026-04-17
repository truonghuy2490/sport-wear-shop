using System;
using System.Collections.Generic;

namespace SportWearShop.Repositories.Entities;

public partial class ProductRating
{
    public long ProductRatingId { get; set; }

    public long ProductId { get; set; }

    public long UserId { get; set; }

    public int RatingValue { get; set; }

    public string? ReviewText { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
