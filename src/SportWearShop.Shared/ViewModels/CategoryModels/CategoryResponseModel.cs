using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.CategoryModels;

public class CategoryResponseModel
{
    public int CategoryId { get; set; }
    public int? ParentCategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategoryCode { get; set; } = null!;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
