using System;
using System.Collections.Generic;
using System.Text;
using SportWearShop.Repositories.Enums;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels;



// Must: include category and brand in the query model to support filtering by category and brand
public class ProductQueryRequestModel
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;

    public string? SearchTerm { get; set; }

    public List<int>? BrandIds { get; set; }
    public List<int>? CategoryIds { get; set; }

    public ProductGender? Gender { get; set; }
    public ProductStatus? Status { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    public bool? IsNewRelease { get; set; }
    public bool? IsOnSale { get; set; }

    public DateTime? CreatedFromUtc { get; set; }
    public DateTime? CreatedToUtc { get; set; }
    public ProductSortBy SortBy { get; set; } = ProductSortBy.CreatedAtUtc;
    public bool IsAscending { get; set; } = false;
}