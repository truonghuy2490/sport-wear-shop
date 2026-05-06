using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.Shared.ViewModels.BrandModels
{
    public class BrandDetailResponseModel
    {
        public int BrandId { get; set; }

        public string BrandName { get; set; } = null!;

        public string BrandCode { get; set; } = null!;

        public string? BrandImage { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public int ProductCount { get; set; }
    }
}
