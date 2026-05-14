namespace SportWearShop.BusinessLogics.ResponseModels.BrandModels;

public class CategoryTreeResponseModel
{
    public int CategoryId { get; set; }

    public int? ParentCategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string CategoryCode { get; set; } = null!;

    public int SortOrder { get; set; }

    public List<CategoryTreeResponseModel> Children { get; set; } = [];
}