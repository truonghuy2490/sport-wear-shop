using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.BrandModels;

public class UpdateBrandRequestModel
{
    public string BrandName { get; set; } = null!;

    public string BrandCode { get; set; } = null!;

    public string? BrandImage { get; set; }

    public bool IsActive { get; set; }
}