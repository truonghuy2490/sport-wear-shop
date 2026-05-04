using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.CategoryModels;

public class CategoryCreateRequestModel
{
    public int? ParentCategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public string CategoryCode { get; set; } = null!;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}