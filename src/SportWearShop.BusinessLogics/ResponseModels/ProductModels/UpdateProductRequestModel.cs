using System;
using System.Collections.Generic;
using System.Text;
using SportWearShop.Repositories.Enums;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels
{
    public class UpdateProductRequestModel
    {
        public string? ProductName { get; set; }

        public string? Slug { get; set; }

        public string? Description { get; set; }

        public ProductGender? Gender { get; set; }

        public string? BaseMaterial { get; set; }

        public ProductStatus? Status { get; set; }

        public int? BrandId { get; set; }

        public int? CategoryId { get; set; }
    }
    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
