using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.BrandModels;

public class CreateBrandRequestModel
{
    public string BrandName { get; set; } = null!;

    public string BrandCode { get; set; } = null!;

    public string? BrandImage { get; set; }
}