using SportWearShop.Repositories.Enums;
using System;
using System.Collections.Generic;

namespace SportWearShop.Repositories.Entities;

public partial class ProductVariant
{
    public long ProductVariantId { get; set; }

    public long ProductId { get; set; }

    public string Sku { get; set; } = null!;

    public string ColorCode { get; set; } = null!;

    public string ColorName { get; set; } = null!;

    public string SizeCode { get; set; } = null!;

    public string SizeLabel { get; set; } = null!;

    public decimal ListPrice { get; set; }

    public decimal? SalePrice { get; set; }

    public int? WeightGrams { get; set; }

    public ProductVariantStatus Status { get; set; } = ProductVariantStatus.Draft;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    public virtual InventoryStock? InventoryStock { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
