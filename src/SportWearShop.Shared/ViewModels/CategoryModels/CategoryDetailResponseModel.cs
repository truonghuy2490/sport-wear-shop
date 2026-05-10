using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.CategoryModels;

public class CategoryDetailResponseModel : CategoryResponseModel
{
    public List<CategoryResponseModel> Children { get; set; } = new();
}