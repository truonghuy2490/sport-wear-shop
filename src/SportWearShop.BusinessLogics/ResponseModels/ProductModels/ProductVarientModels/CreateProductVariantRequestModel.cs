using System;
using System.Collections.Generic;
using System.Text;

namespace SportWearShop.BusinessLogics.ResponseModels.ProductModels.ProductVarientModels
{
    public class CreateProductVariantRequestModel
    {
        public string Sku { get; set; } = string.Empty;

        public string ColorCode { get; set; } = string.Empty;

        public string ColorName { get; set; } = string.Empty;

        public string SizeCode { get; set; } = string.Empty;

        public string SizeLabel { get; set; } = string.Empty;

        public decimal ListPrice { get; set; }

        public decimal? SalePrice { get; set; }

        public int? WeightGrams { get; set; }

        public string Status { get; set; } = "ACTIVE";

        public int InitialStockQuantity { get; set; } = 0;
    }
}
