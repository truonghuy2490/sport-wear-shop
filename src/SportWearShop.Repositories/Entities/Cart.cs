using System;
using System.Collections.Generic;

namespace SportWearShop.Repositories.Entities;

public partial class Cart
{
    public long CartId { get; set; }

    public long UserId { get; set; }

    public string CartStatus { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual AppUser User { get; set; } = null!;
}
